using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;
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
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {FullName}")]
    public partial class MethodDefinition : MethodInfo, IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<MethodDebugInformation> _debugInformation;
        private readonly Lazy<ImmutableArray<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<TypeDefinition> _declaringType;
        private readonly Lazy<string> _fullName;
        private readonly Lazy<IEnumerable<GenericParameter>> _genericParameters;
        private readonly Lazy<MethodImport> _import;
        private readonly Lazy<MethodBodyToExpose> _methodBody;
        private readonly Lazy<ImmutableArray<ParameterInfoToExpose>> _parameters;
        private readonly Lazy<EventInfo> _relatedEvent;
        private readonly Lazy<PropertyInfo> _relatedProperty;
        private readonly Lazy<Parameter> _returnParameter;
        private readonly Lazy<MethodSignature<TypeBase>> _signature;

        internal MethodDefinition(MethodDefinitionHandle metadataHandle, MetadataState metadataState)
        {
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.AssemblyReader.GetMethodDefinition(metadataHandle);
            Key = new CodeElementKey<MethodDefinition>(metadataHandle);
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            Attributes = RawMetadata.Attributes;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            _declaringType = MetadataState.GetLazyCodeElement<TypeDefinition>(RawMetadata.GetDeclaringType());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(RawMetadata.GetDeclarativeSecurityAttributes());
            _import = MetadataState.GetLazyCodeElement<MethodImport>(RawMetadata.GetImport());
            _methodBody = new Lazy<MethodBodyToExpose>(() => RawMetadata.RelativeVirtualAddress == 0 ? null : MetadataState.GetCodeElement<MethodBody>(new CodeElementKey<MethodBody>(RawMetadata.RelativeVirtualAddress)));
            _parameters = new Lazy<ImmutableArray<ParameterInfoToExpose>>(LoadParameters);
            _debugInformation = new Lazy<MethodDebugInformation>(() => !MetadataState.HasDebugMetadata ? null : MetadataState.GetCodeElement<MethodDebugInformation>(metadataHandle.ToDebugInformationHandle(), this, new GenericContext(this, _declaringType.Value.GenericTypeParameters, _genericParameters.Value)));
            _signature = new Lazy<MethodSignature<TypeBase>>(() => RawMetadata.DecodeSignature(MetadataState.TypeProvider, new GenericContext(this, _declaringType.Value.GenericTypeParameters, _genericParameters.Value)));

            _genericParameters = new Lazy<IEnumerable<GenericParameter>>(() =>
            {
                var genericParameters = MetadataState.GetCodeElements<GenericParameter>(RawMetadata.GetGenericParameters());
                foreach (var genericParameter in genericParameters)
                {
                    genericParameter.SetDeclaringMethod(this);
                    genericParameter.SetDeclaringType((TypeBase)DeclaringType);
                }

                return genericParameters;
            });
            _fullName = new Lazy<string>(() =>
            {
                var basicName = $"{DeclaringType.FullName}.{Name}";
                var genericParameters = _genericParameters.Value.Any() ? $"`{_genericParameters.Value.Count()}" : string.Empty;
                var parameters = !_relatedProperty.Value?.IsIndexer == true || RelatedEvent != null ? string.Empty : $"({string.Join(", ", GetParameters().Select(parameter => parameter.ParameterType.IsGenericParameter ? parameter.ParameterType.Name : ((TypeBase)parameter.ParameterType).FullNameWithoutAssemblies))})";

                return basicName + genericParameters + parameters;
            });
            // ReSharper disable once RedundantEnumerableCastCall
            _relatedEvent = new Lazy<EventInfo>(() => ((TypeBase)DeclaringType).DeclaredEvents.Cast<EventInfo>().SingleOrDefault(@event => @event.AddMethod == this || @event.RemoveMethod == this || @event.RemoveMethod == this));
            // ReSharper disable once RedundantEnumerableCastCall
            _relatedProperty = new Lazy<PropertyInfo>(() => ((TypeBase)DeclaringType).DeclaredProperties.Cast<PropertyInfo>().SingleOrDefault(property => property.GetMethod == this || property.SetMethod == this));
            _returnParameter = MetadataState.GetLazyCodeElement<Parameter>(this, Signature.ReturnType);
        }

        public override MethodAttributes Attributes { get; }

        public override CallingConventions CallingConvention => Signature.Header.CallingConvention == SignatureCallingConvention.Default ? CallingConventions.Standard | (IsStatic ? 0 : CallingConventions.HasThis) : throw new ArgumentException($"Unable to handle the signature calling convention {Signature.Header.CallingConvention}");

        public override bool ContainsGenericParameters => _genericParameters.Value.Any() || DeclaringType.ContainsGenericParameters;

        public MethodDebugInformation DebugInformation => _debugInformation.Value;

        public ImmutableArray<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public override TypeToExpose DeclaringType => _declaringType.Value;

        public Document Document => DebugInformation?.Document;

        public override string FullName => _fullName.Value;

        public MethodImport Import => _import.Value;

        public override bool IsGenericMethod => _genericParameters.Value.Any();

        public override bool IsGenericMethodDefinition => _genericParameters.Value.Any(parameter => parameter.IsGenericParameter);

        public override int MetadataToken => Key.Handle.Value.GetHashCode();

        public override ModuleToExpose Module => MetadataState.ModuleDefinition;

        public override string Name { get; }

        public override IEnumerable<ParameterInfoToExpose> Parameters => _parameters.Value;

        public override TypeToExpose ReflectedType => throw NotSupportedHelper.FutureVersion();

        public override EventInfoToExpose RelatedEvent => _relatedEvent.Value;

        public override PropertyInfoToExpose RelatedProperty => _relatedProperty.Value;

        public override ParameterInfoToExpose ReturnParameter => _returnParameter.Value;

        public override TypeToExpose ReturnType => Signature.ReturnType;

        public string SourceCode => DebugInformation?.SourceCode;

        public override string TextSignature => FullName;

        internal CodeElementKey Key { get; }

        internal MethodDefinitionHandle MetadataHandle { get; }

        internal MetadataState MetadataState { get; }

        internal System.Reflection.Metadata.MethodDefinition RawMetadata { get; }

        internal MethodSignature<TypeBase> Signature => _signature.Value;

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<CustomAttributeDataToExpose>();

        public override Type[] GetGenericArguments() => _genericParameters.Value.ToArray<Type>();

        public override MethodBodyToExpose GetMethodBody() => _methodBody.Value;

        public override MethodImplAttributes GetMethodImplementationFlags() => RawMetadata.ImplAttributes;

        public override ParameterInfoToExpose[] GetParameters() => _parameters.Value.ToArray();

        private ImmutableArray<ParameterInfoToExpose> LoadParameters()
        {
            IEnumerable<ParameterInfoToExpose> parameters;
            var parameterHandles = RawMetadata.GetParameters();
            if (Signature.ParameterTypes.Any() && parameterHandles.Count == 0)
            {
                /* Parameters do not have to be named in IL.  When this happens, the parameter does not show up in the parameter list but will have a parameter type.  If there are no parameters listed,
                 * we can use the order of the parameter types to get the position. -- Jonathan Byrne 01/11/2017
                */
                parameters = Signature.ParameterTypes.Select((parameterType, position) => new Parameter(this, parameterType, position, position > Signature.RequiredParameterCount, MetadataState)).Cast<ParameterInfoToExpose>().ToImmutableArray();
            }
            else if (Signature.ParameterTypes.Length > parameterHandles.Count)
            {
                parameters = Signature.ParameterTypes.Select((parameterType, position) => new Parameter(this, parameterType, position, position > Signature.RequiredParameterCount, MetadataState)).Cast<ParameterInfoToExpose>().ToImmutableArray();
                /* 
                 * TODO: After logging is enabled, log this message:
                 *  $"Method {DeclaringType.FullName}.{Name} has {parameterHandles.Count} parameters but {Signature.ParameterTypes.Length} parameter types were found"
                 */
            }
            else if (Signature.ParameterTypes.Length < parameterHandles.Count)
            {
                var parameterList = new List<ParameterInfoToExpose>();
                foreach (var parameterHandle in parameterHandles)
                {
                    var parameter = MetadataState.AssemblyReader.GetParameter(parameterHandle);
                    if (parameter.SequenceNumber > 0)
                    {
                        parameterList.Add(MetadataState.GetCodeElement<Parameter>(parameterHandle, this));
                    }
                }

                parameters = parameterList.ToImmutableArray();
            }
            else
            {
                parameters = parameterHandles.Select(parameterHandle => MetadataState.GetCodeElement<Parameter>(parameterHandle, this)).Cast<ParameterInfoToExpose>().ToImmutableArray();
            }

            return parameters.ToImmutableArray();
        }
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public partial class MethodDefinition
    {
        public override RuntimeMethodHandle MethodHandle => throw NotSupportedHelper.NotValidForMetadata();

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw NotSupportedHelper.FutureVersion();

        public override MethodInfoToExpose GetBaseDefinition() => throw NotSupportedHelper.FutureVersion();

        public override object[] GetCustomAttributes(bool inherit) => CustomAttributeData.GetCustomAttributes(this, inherit);

        public override object[] GetCustomAttributes(TypeToExpose attributeType, bool inherit) => CustomAttributeData.GetCustomAttributes(this, attributeType, inherit);

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw NotSupportedHelper.NotValidForMetadata();

        public override bool IsDefined(TypeToExpose attributeType, bool inherit) => CustomAttributeData.IsDefined(this, attributeType, inherit);
    }
#else
    public partial class MethodDefinition
    {
    }
#endif
}
