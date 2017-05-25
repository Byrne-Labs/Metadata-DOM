using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using System.Globalization;
using TypeInfoToExpose = System.Reflection.TypeInfo;
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using MethodInfoToExpose = System.Reflection.MethodInfo;
using PropertyInfoToExpose = System.Reflection.PropertyInfo;
using ModuleToExpose = System.Reflection.Module;
using EventInfoToExpose = System.Reflection.EventInfo;
using ParameterInfoToExpose = System.Reflection.ParameterInfo;
using MethodBodyToExpose = System.Reflection.MethodBody;

#else
using TypeInfoToExpose = ByrneLabs.Commons.MetadataDom.TypeInfo;
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using MethodInfoToExpose = ByrneLabs.Commons.MetadataDom.MethodInfo;
using PropertyInfoToExpose = ByrneLabs.Commons.MetadataDom.PropertyInfo;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;
using EventInfoToExpose = ByrneLabs.Commons.MetadataDom.EventInfo;
using ParameterInfoToExpose = ByrneLabs.Commons.MetadataDom.ParameterInfo;
using MethodBodyToExpose = ByrneLabs.Commons.MetadataDom.MethodBody;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public partial class MethodDefinition : MethodInfo, IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttributeDataToExpose>> _customAttributes;
        private readonly Lazy<MethodDebugInformation> _debugInformation;
        private readonly Lazy<ImmutableArray<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<TypeInfoToExpose> _declaringType;
        private readonly Lazy<string> _fullName;
        private readonly Lazy<ImmutableArray<GenericParameter>> _genericParameters;
        private readonly Lazy<MethodImport> _import;
        private readonly Lazy<MethodBodyToExpose> _methodBody;
        private readonly Lazy<ImmutableArray<ParameterInfoToExpose>> _parameters;
        private readonly Lazy<EventInfo> _relatedEvent;
        private readonly Lazy<PropertyInfo> _relatedProperty;
        private readonly Lazy<MethodSignature<TypeBase>> _signature;

        internal MethodDefinition(MethodDefinitionHandle metadataHandle, MetadataState metadataState)
        {
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.AssemblyReader.GetMethodDefinition(metadataHandle);
            Key = new CodeElementKey<MethodDefinition>(metadataHandle);
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            Attributes = RawMetadata.Attributes;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttributeDataToExpose>(RawMetadata.GetCustomAttributes());
            _declaringType = MetadataState.GetLazyCodeElement<TypeInfoToExpose>(RawMetadata.GetDeclaringType());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(RawMetadata.GetDeclarativeSecurityAttributes());
            _import = MetadataState.GetLazyCodeElement<MethodImport>(RawMetadata.GetImport());
            _methodBody = new Lazy<MethodBodyToExpose>(() => RawMetadata.RelativeVirtualAddress == 0 ? null : MetadataState.GetCodeElement<MethodBody>(new CodeElementKey<MethodBody>(RawMetadata.RelativeVirtualAddress)));
            _parameters = new Lazy<ImmutableArray<ParameterInfoToExpose>>(LoadParameters);
            _debugInformation = new Lazy<MethodDebugInformation>(() => !MetadataState.HasDebugMetadata ? null : MetadataState.GetCodeElement<MethodDebugInformation>(metadataHandle.ToDebugInformationHandle()));
            _signature = new Lazy<MethodSignature<TypeBase>>(() => RawMetadata.DecodeSignature(MetadataState.TypeProvider, new GenericContext(this, _declaringType.Value.GenericTypeParameters, _genericParameters.Value)));

            _genericParameters = new Lazy<ImmutableArray<GenericParameter>>(() =>
            {
                var genericParameters = MetadataState.GetCodeElements<GenericParameter>(RawMetadata.GetGenericParameters());
                foreach (var genericParameter in genericParameters)
                {
                    genericParameter.DeclaringMethod = this;
                    genericParameter.SetDeclaringType((TypeBase) DeclaringType);
                }

                return genericParameters;
            });
            _fullName = new Lazy<string>(() =>
            {
                var basicName = $"{DeclaringType.FullName}.{Name}";
                var genericParameters = _genericParameters.Value.Any() ? $"<{string.Join(", ", _genericParameters.Value.Select(genericTypeParameter => genericTypeParameter.Name))}>" : string.Empty;
                var parameters = !_relatedProperty.Value?.IsIndexer == true || RelatedEvent != null ? string.Empty : $"({string.Join(", ", GetParameters().Select(parameter => parameter.ParameterType.IsGenericParameter ? parameter.ParameterType.Name : ((TypeBase) parameter.ParameterType).FullNameWithoutAssemblies))})";

                return basicName + genericParameters + parameters;
            });
            // ReSharper disable once RedundantEnumerableCastCall
            _relatedEvent = new Lazy<EventInfo>(() => ((TypeBase) DeclaringType).DeclaredEvents.Cast<EventInfo>().SingleOrDefault(@event => @event.AddMethod == this || @event.RemoveMethod == this || @event.RemoveMethod == this));
            // ReSharper disable once RedundantEnumerableCastCall
            _relatedProperty = new Lazy<PropertyInfo>(() => ((TypeBase) DeclaringType).DeclaredProperties.Cast<PropertyInfo>().SingleOrDefault(property => property.GetMethod == this || property.SetMethod == this));
        }

        public override MethodAttributes Attributes { get; }

        public override bool ContainsGenericParameters { get; }

        public MethodDebugInformation DebugInformation => _debugInformation.Value;

        public ImmutableArray<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public override TypeToExpose DeclaringType => _declaringType.Value;

        public Document Document => DebugInformation?.Document;

        public override string FullName => _fullName.Value;

        public MethodImport Import => _import.Value;

        public override bool IsGenericMethod => _genericParameters.Value.Any();

        public override bool IsGenericMethodDefinition { get; }

        public override int MetadataToken => MetadataHandle.GetHashCode();

        public override ModuleToExpose Module => MetadataState.ModuleDefinition;

        public override string Name { get; }

        public override IEnumerable<ParameterInfoToExpose> Parameters => _parameters.Value;

        public override TypeToExpose ReflectedType => throw new NotSupportedException();

        public override EventInfoToExpose RelatedEvent => _relatedEvent.Value;

        public override PropertyInfoToExpose RelatedProperty => _relatedProperty.Value;

        public override TypeInfoToExpose ReturnType => Signature.ReturnType;

        public string SourceCode => DebugInformation?.SourceCode;

        public override string TextSignature => FullName;

        internal CodeElementKey Key { get; }

        internal MethodDefinitionHandle MetadataHandle { get; }

        internal MetadataState MetadataState { get; }

        internal System.Reflection.Metadata.MethodDefinition RawMetadata { get; }

        internal MethodSignature<TypeBase> Signature => _signature.Value;

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList();

        public override Type[] GetGenericArguments() => _genericParameters.Value.ToArray<Type>();

        public override MethodBodyToExpose GetMethodBody() => _methodBody.Value;

        public override MethodImplAttributes GetMethodImplementationFlags() => RawMetadata.ImplAttributes;

        public override ParameterInfoToExpose[] GetParameters() => _parameters.Value.ToArray();

        private ImmutableArray<ParameterInfoToExpose> LoadParameters()
        {
            ImmutableArray<ParameterInfoToExpose> parameters;
            var parameterHandles = RawMetadata.GetParameters();
            if (Signature.ParameterTypes.Any() && parameterHandles.Count == 0)
            {
                /* Parameters do not have to be named in IL.  When this happens, the parameter does not show up in the parameter list but will have a parameter type.  If there are no parameters listed,
                 * we can use the order of the parameter types to get the position. -- Jonathan Byrne 01/11/2017
                */
                parameters = Signature.ParameterTypes.Select((parameterType, position) => new Parameter(this, parameterType, position, position > Signature.RequiredParameterCount, MetadataState)).Cast<ParameterInfoToExpose>().ToImmutableArray();
            }
            else if (Signature.ParameterTypes.Length != parameterHandles.Count)
            {
                throw new BadMetadataException($"Method {DeclaringType.FullName}.{Name} has {parameterHandles.Count} parameters but {Signature.ParameterTypes.Length} parameter types were found");
            }
            else
            {
                parameters = parameterHandles.Select(parameterHandle => MetadataState.GetCodeElement<Parameter>(parameterHandle, this)).Cast<ParameterInfoToExpose>().ToImmutableArray();
            }

            return parameters;
        }
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public partial class MethodDefinition
    {
        public override RuntimeMethodHandle MethodHandle => throw new NotSupportedException();

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw new NotSupportedException();

        public override MethodInfoToExpose GetBaseDefinition() => throw new NotImplementedException();

        public override object[] GetCustomAttributes(bool inherit) => throw new NotImplementedException();

        public override object[] GetCustomAttributes(TypeToExpose attributeType, bool inherit) => throw new NotImplementedException();

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw new NotImplementedException();

        public override bool IsDefined(TypeToExpose attributeType, bool inherit) => throw new NotImplementedException();
    }
#else
    public partial class MethodDefinition
    {
    }
#endif
}
