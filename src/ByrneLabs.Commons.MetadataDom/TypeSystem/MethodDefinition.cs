using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {FullName}")]
    public class MethodDefinition : MethodInfo, IManagedCodeElement
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<MethodDebugInformation> _debugInformation;
        private readonly Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<TypeDefinition> _declaringType;
        private readonly Lazy<string> _fullName;
        private readonly Lazy<IEnumerable<GenericParameter>> _genericParameters;
        private readonly Lazy<MethodImport> _import;
        private readonly Lazy<MethodBody> _methodBody;
        private readonly Lazy<IEnumerable<System.Reflection.ParameterInfo>> _parameters;
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
            _methodBody = new Lazy<MethodBody>(() => RawMetadata.RelativeVirtualAddress == 0 ? null : MetadataState.GetCodeElement<MethodBody>(new CodeElementKey<MethodBody>(RawMetadata.RelativeVirtualAddress)));
            _parameters = new Lazy<IEnumerable<System.Reflection.ParameterInfo>>(LoadParameters);
            _debugInformation = new Lazy<MethodDebugInformation>(() => !MetadataState.HasDebugMetadata ? null : MetadataState.GetCodeElement<MethodDebugInformation>(metadataHandle.ToDebugInformationHandle(), this, new GenericContext(this, _declaringType.Value.GenericTypeParameters, _genericParameters.Value)));
            _signature = new Lazy<MethodSignature<TypeBase>>(() => RawMetadata.DecodeSignature(MetadataState.TypeProvider, new GenericContext(this, _declaringType.Value.GenericTypeParameters, _genericParameters.Value)));

            _genericParameters = new Lazy<IEnumerable<GenericParameter>>(() =>
            {
                var genericParameters = MetadataState.GetCodeElements<GenericParameter>(RawMetadata.GetGenericParameters());
                foreach (var genericParameter in genericParameters)
                {
                    genericParameter.SetDeclaringMethod(this);
                    genericParameter.SetDeclaringType((TypeBase) DeclaringType);
                }

                return genericParameters;
            });
            _fullName = new Lazy<string>(() =>
            {
                var basicName = $"{DeclaringType.FullName}.{Name}";
                var genericParameters = _genericParameters.Value.Any() ? $"`{_genericParameters.Value.Count()}" : string.Empty;
                var parameters = !_relatedProperty.Value?.IsIndexer == true || RelatedEvent != null ? string.Empty : $"({string.Join(", ", GetParameters().Select(parameter => parameter.ParameterType.IsGenericParameter ? parameter.ParameterType.Name : ((TypeBase) parameter.ParameterType).FullNameWithoutAssemblies))})";

                return basicName + genericParameters + parameters;
            });
            // ReSharper disable once RedundantEnumerableCastCall
            _relatedEvent = new Lazy<EventInfo>(() => ((TypeBase) DeclaringType).DeclaredEvents.Cast<EventInfo>().SingleOrDefault(@event => @event.AddMethod == this || @event.RemoveMethod == this || @event.RemoveMethod == this));
            // ReSharper disable once RedundantEnumerableCastCall
            _relatedProperty = new Lazy<PropertyInfo>(() => ((TypeBase) DeclaringType).DeclaredProperties.Cast<PropertyInfo>().SingleOrDefault(property => property.GetMethod == this || property.SetMethod == this));
            _returnParameter = MetadataState.GetLazyCodeElement<Parameter>(this, Signature.ReturnType);
        }

        public override MethodAttributes Attributes { get; }

        public override CallingConventions CallingConvention => Signature.Header.CallingConvention == SignatureCallingConvention.Default ? CallingConventions.Standard | (IsStatic ? 0 : CallingConventions.HasThis) : throw new ArgumentException($"Unable to handle the signature calling convention {Signature.Header.CallingConvention}");

        public override bool ContainsGenericParameters => _genericParameters.Value.Any() || DeclaringType.ContainsGenericParameters;

        public MethodDebugInformation DebugInformation => _debugInformation.Value;

        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public override Type DeclaringType => _declaringType.Value;

        public Document Document => DebugInformation?.Document;

        public override string FullName => _fullName.Value;

        public MethodImport Import => _import.Value;

        public override bool IsGenericMethod => _genericParameters.Value.Any();

        public override bool IsGenericMethodDefinition => _genericParameters.Value.Any(parameter => parameter.IsGenericParameter);

        public override int MetadataToken => Key.Handle.Value.GetHashCode();

        public override RuntimeMethodHandle MethodHandle => throw NotSupportedHelper.NotValidForMetadata();

        public override System.Reflection.Module Module => MetadataState.ModuleDefinition;

        public override string Name { get; }

        public override IEnumerable<System.Reflection.ParameterInfo> Parameters => _parameters.Value;

        public override Type ReflectedType => throw NotSupportedHelper.FutureVersion();

        public override EventInfo RelatedEvent => _relatedEvent.Value;

        public override PropertyInfo RelatedProperty => _relatedProperty.Value;

        public override System.Reflection.ParameterInfo ReturnParameter => _returnParameter.Value;

        public override Type ReturnType => Signature.ReturnType;

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw NotSupportedHelper.FutureVersion();

        public override IEnumerable<MetadataDom.SequencePoint> SequencePoints => DebugInformation?.SequencePoints.Cast<MetadataDom.SequencePoint>().ToImmutableArray();

        public override string SourceCode => DebugInformation?.SourceCode;

        public override string TextSignature => FullName;

        internal CodeElementKey Key { get; }

        internal MethodDefinitionHandle MetadataHandle { get; }

        internal MetadataState MetadataState { get; }

        internal System.Reflection.Metadata.MethodDefinition RawMetadata { get; }

        internal MethodSignature<TypeBase> Signature => _signature.Value;

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override System.Reflection.MethodInfo GetBaseDefinition() => throw NotSupportedHelper.FutureVersion();

        public override object[] GetCustomAttributes(bool inherit) => CustomAttributeData.GetCustomAttributes(this, inherit);

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => CustomAttributeData.GetCustomAttributes(this, attributeType, inherit);

        public override IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<System.Reflection.CustomAttributeData>();

        public override Type[] GetGenericArguments() => _genericParameters.Value.ToArray<Type>();

        public override System.Reflection.MethodBody GetMethodBody() => _methodBody.Value;

        public override MethodImplAttributes GetMethodImplementationFlags() => RawMetadata.ImplAttributes;

        public override System.Reflection.ParameterInfo[] GetParameters() => _parameters.Value.ToArray();

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw NotSupportedHelper.NotValidForMetadata();

        public override bool IsDefined(Type attributeType, bool inherit) => CustomAttributeData.IsDefined(this, attributeType, inherit);

        private IEnumerable<System.Reflection.ParameterInfo> LoadParameters()
        {
            IEnumerable<ParameterInfo> parameters;
            var parameterHandles = RawMetadata.GetParameters();
            if (Signature.ParameterTypes.Any() && parameterHandles.Count == 0)
            {
                /* Parameters do not have to be named in IL.  When this happens, the parameter does not show up in the parameter list but will have a parameter type.  If there are no parameters listed,
                 * we can use the order of the parameter types to get the position. -- Jonathan Byrne 01/11/2017
                */
                parameters = Signature.ParameterTypes.Select((parameterType, position) => new Parameter(this, parameterType, position, position > Signature.RequiredParameterCount, MetadataState)).Cast<ParameterInfo>().ToImmutableArray();
            }
            else if (Signature.ParameterTypes.Length > parameterHandles.Count)
            {
                parameters = Signature.ParameterTypes.Select((parameterType, position) => new Parameter(this, parameterType, position, position > Signature.RequiredParameterCount, MetadataState)).Cast<ParameterInfo>().ToImmutableArray();
                /* 
                 * TODO: After logging is enabled, log this message:
                 *  $"Method {DeclaringType.FullName}.{Name} has {parameterHandles.Count} parameters but {Signature.ParameterTypes.Length} parameter types were found"
                 */
            }
            else if (Signature.ParameterTypes.Length < parameterHandles.Count)
            {
                var parameterList = (from parameterHandle in parameterHandles
                        let parameter = MetadataState.AssemblyReader.GetParameter(parameterHandle)
                        where parameter.SequenceNumber > 0
                        select MetadataState.GetCodeElement<Parameter>(parameterHandle, this)).Cast<ParameterInfo>()
                    .ToList();

                parameters = parameterList.ToImmutableArray();
            }
            else
            {
                parameters = parameterHandles.Select(parameterHandle => MetadataState.GetCodeElement<Parameter>(parameterHandle, this)).Cast<ParameterInfo>().ToImmutableArray();
            }

            return parameters.ToImmutableArray<System.Reflection.ParameterInfo>();
        }
    }
}
