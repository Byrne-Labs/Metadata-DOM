using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using TypeInfoToExpose = System.Reflection.TypeInfo;
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using ModuleToExpose = System.Reflection.Module;
using ParameterInfoToExpose = System.Reflection.ParameterInfo;
using System.Globalization;
using MethodBodyToExpose = System.Reflection.MethodBody;

#else
using TypeInfoToExpose = ByrneLabs.Commons.MetadataDom.TypeInfo;
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;
using ParameterInfoToExpose = ByrneLabs.Commons.MetadataDom.ParameterInfo;
using MethodBodyToExpose = ByrneLabs.Commons.MetadataDom.MethodBody;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public partial class ConstructorDefinition : ConstructorInfo, IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttributeData>> _customAttributes;
        private readonly Lazy<MethodDebugInformation> _debugInformation;
        private readonly Lazy<ImmutableArray<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<TypeInfoToExpose> _declaringType;
        private readonly Lazy<string> _fullName;
        private readonly Lazy<MethodImport> _import;
        private readonly Lazy<MethodBody> _methodBody;
        private readonly Lazy<ImmutableArray<ParameterInfoToExpose>> _parameters;
        private readonly Lazy<MethodSignature<TypeBase>> _signature;

        internal ConstructorDefinition(MethodDefinitionHandle metadataHandle, MetadataState metadataState)
        {
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            Key = new CodeElementKey<ConstructorDefinition>(metadataHandle);
            RawMetadata = MetadataState.AssemblyReader.GetMethodDefinition(metadataHandle);
            _fullName = new Lazy<string>(() =>
            {
                var basicName = $"{DeclaringType.FullName}{Name}";
                var genericParameters = GenericTypeParameters.Any() ? $"<{string.Join(", ", GenericTypeParameters.Select(genericTypeParameter => genericTypeParameter.Name))}>" : string.Empty;
                var parameters = $"({string.Join(", ", GetParameters().Select(parameter => parameter.ParameterType.IsGenericParameter ? parameter.ParameterType.Name : ((TypeBase) parameter.ParameterType).FullNameWithoutAssemblies))})";

                return basicName + genericParameters + parameters;
            });
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttributeData>(RawMetadata.GetCustomAttributes());
            _declaringType = MetadataState.GetLazyCodeElement<TypeInfoToExpose>(RawMetadata.GetDeclaringType());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(RawMetadata.GetDeclarativeSecurityAttributes());
            _import = MetadataState.GetLazyCodeElement<MethodImport>(RawMetadata.GetImport());
            _methodBody = new Lazy<MethodBody>(() => RawMetadata.RelativeVirtualAddress == 0 ? null : MetadataState.GetCodeElement<MethodBody>(new CodeElementKey<MethodBody>(RawMetadata.RelativeVirtualAddress)));
            _parameters = new Lazy<ImmutableArray<ParameterInfoToExpose>>(LoadParameters);
            _debugInformation = new Lazy<MethodDebugInformation>(() => !MetadataState.HasDebugMetadata ? null : MetadataState.GetCodeElement<MethodDebugInformation>(metadataHandle.ToDebugInformationHandle()));
            _signature = new Lazy<MethodSignature<TypeBase>>(() => RawMetadata.DecodeSignature(MetadataState.TypeProvider, new GenericContext(this, _declaringType.Value.GenericTypeParameters, GenericTypeParameters)));
        }

        public override MethodAttributes Attributes => RawMetadata.Attributes;

        public override bool ContainsGenericParameters => false;

        public MethodDebugInformation DebugInformation => _debugInformation.Value;

        public ImmutableArray<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public override TypeToExpose DeclaringType => _declaringType.Value;

        public Document Document => DebugInformation?.Document;

        public override string FullName => _fullName.Value;

        public IEnumerable<TypeInfoToExpose> GenericTypeParameters { get; } = ImmutableArray<TypeInfo>.Empty;

        public MethodImport Import => _import.Value;

        public override bool IsGenericMethod => false;

        public override bool IsGenericMethodDefinition => false;

        public override MemberTypes MemberType => MemberTypes.Constructor;

        public MethodDefinitionHandle MetadataHandle { get; }

        public override int MetadataToken => MetadataHandle.GetHashCode();

        public override ModuleToExpose Module => MetadataState.ModuleDefinition;

        public override string Name { get; }

        public override IEnumerable<ParameterInfoToExpose> Parameters => _parameters.Value.ToImmutableList();

        public System.Reflection.Metadata.MethodDefinition RawMetadata { get; }

        public override Type ReflectedType => null;

        public string SourceCode => DebugInformation?.SourceCode;

        public override string TextSignature => FullName;

        protected MethodSignature<TypeBase> Signature => _signature.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<CustomAttributeDataToExpose>();

        public override TypeToExpose[] GetGenericArguments() => new Type[] { };

        public override MethodBodyToExpose GetMethodBody() => _methodBody.Value;

        public override MethodImplAttributes GetMethodImplementationFlags() => RawMetadata.ImplAttributes;

        public override ParameterInfoToExpose[] GetParameters() => _parameters.Value.ToArray();

        private ImmutableArray<ParameterInfoToExpose> LoadParameters()
        {
            var allParameters = MetadataState.GetCodeElements<Parameter>(RawMetadata.GetParameters()).ToList();
            var parameters = allParameters.Where(parameter => parameter.Position >= 0).ToList();
            if (Signature.ParameterTypes.Any() && parameters.Count == 0)
            {
                /* Parameters do not have to be named in IL.  When this happens, the parameter does not show up in the parameter list but will have a parameter type.  If there are no parameters listed,
                 * we can use the order of the parameter types to get the position. -- Jonathan Byrne 01/11/2017
                */
                parameters.AddRange(Signature.ParameterTypes.Select((parameterType, position) => new Parameter(this, parameterType, position, position > Signature.RequiredParameterCount, MetadataState)));
            }
            else
            {
                if (Signature.ParameterTypes.Length != parameters.Count)
                {
                    throw new BadMetadataException($"Method {DeclaringType.FullName}.{Name} has {parameters.Count} parameters but {Signature.ParameterTypes.Length} parameter types were found");
                }

                for (var position = 0; position < parameters.Count; position++)
                {
                    if (!parameters.Exists(parameter => parameter.Position == position))
                    {
                        throw new BadMetadataException($"Method {DeclaringType.FullName}{Name} has {parameters.Count} parameters but does not have a parameter for position {position}");
                    }
                }
            }

            return parameters.Cast<ParameterInfoToExpose>().ToImmutableArray();
        }
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public partial class ConstructorDefinition
    {
        public override RuntimeMethodHandle MethodHandle => throw new NotSupportedException();

        public override object[] GetCustomAttributes(bool inherit) => throw new NotSupportedException();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotSupportedException();

        public override bool IsDefined(TypeToExpose attributeType, bool inherit) => throw new NotSupportedException();

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw new NotImplementedException();

        public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw new NotSupportedException();
    }

#else
    public partial class ConstructorDefinition
    {
    }

#endif
}
