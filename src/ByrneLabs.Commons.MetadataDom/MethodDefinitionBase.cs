using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition" />
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {TextSignature}")]
    //[PublicAPI]
    public abstract class MethodDefinitionBase : MethodBase<MethodDefinitionBase, MethodDefinitionHandle, System.Reflection.Metadata.MethodDefinition>, IContainsSourceCode
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<MethodDebugInformation> _debugInformation;
        private readonly Lazy<ImmutableArray<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<TypeDefinition> _declaringType;
        private readonly Lazy<MethodImport> _import;
        private readonly Lazy<MethodBody> _methodBody;
        private readonly Lazy<ImmutableArray<IParameter>> _parameters;
        private readonly Lazy<MethodSignature<TypeBase>> _signature;

        internal MethodDefinitionBase(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            Name = AsString(RawMetadata.Name);
            Attributes = RawMetadata.Attributes;
            MethodImplementationFlags = RawMetadata.ImplAttributes;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            _declaringType = MetadataState.GetLazyCodeElement<TypeDefinition>(RawMetadata.GetDeclaringType());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(RawMetadata.GetDeclarativeSecurityAttributes());
            _import = MetadataState.GetLazyCodeElement<MethodImport>(RawMetadata.GetImport());
            _methodBody = new Lazy<MethodBody>(() => RawMetadata.RelativeVirtualAddress == 0 ? null : MetadataState.GetCodeElement<MethodBody>(new CodeElementKey<MethodBody>(RawMetadata.RelativeVirtualAddress)));
            _parameters = new Lazy<ImmutableArray<IParameter>>(LoadParameters);
            _debugInformation = new Lazy<MethodDebugInformation>(() => !MetadataState.HasDebugMetadata ? null : MetadataState.GetCodeElement<MethodDebugInformation>(metadataHandle.ToDebugInformationHandle()));
            _signature = new Lazy<MethodSignature<TypeBase>>(() => RawMetadata.DecodeSignature(MetadataState.TypeProvider, new GenericContext(_declaringType.Value.GenericTypeParameters, GenericTypeParameters)));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.Attributes" />
        public MethodAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetCustomAttributes" />
        public override ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinitionHandle.ToDebugInformationHandle" />
        /// <summary>Returns a <see cref="MethodDebugInformation" /> corresponding to this handle.</summary>
        /// <remarks></remarks>
        public MethodDebugInformation DebugInformation => _debugInformation.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetDeclarativeSecurityAttributes" />
        public ImmutableArray<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetDeclaringType" />
        public override TypeBase DeclaringType => _declaringType.Value;

        public override string FullName => $"{DeclaringType.FullName}.{Name}({string.Join(", ", Parameters.Select(parameter => parameter.ParameterType.FullName))})";

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetImport" />
        public MethodImport Import => _import.Value;

        public bool IsAbstract => Attributes.HasFlag(MethodAttributes.Abstract);

        public bool IsAssembly => Attributes.HasFlag(MethodAttributes.Assembly);

        public bool IsEventAdder => IsSpecialName && Name.StartsWith("add_");

        public bool IsEventRaiser => IsSpecialName && Name.StartsWith("raise_");

        public bool IsEventRemover => IsSpecialName && Name.StartsWith("remove_");

        public bool IsFamily => Attributes.HasFlag(MethodAttributes.Family) && !IsPublic && !IsFamilyOrAssembly;

        public bool IsFamilyAndAssembly => Attributes.HasFlag(MethodAttributes.FamANDAssem) && !IsAssembly && !IsPublic;

        public bool IsFamilyOrAssembly => Attributes.HasFlag(MethodAttributes.FamORAssem) && !IsPublic;

        public bool IsFinal => Attributes.HasFlag(MethodAttributes.Final);

        public bool IsHideBySig => Attributes.HasFlag(MethodAttributes.HideBySig);

        public bool IsPrivate => Attributes.HasFlag(MethodAttributes.Private) && !IsAssembly && !IsFamilyOrAssembly;

        public bool IsPropertyGetter => IsSpecialName && Name.StartsWith("get_");

        public bool IsPropertySetter => IsSpecialName && Name.StartsWith("set_");

        public bool IsPublic => Attributes.HasFlag(MethodAttributes.Public);

        public bool IsSpecialName => Attributes.HasFlag(MethodAttributes.SpecialName);

        public bool IsStatic => Attributes.HasFlag(MethodAttributes.Static);

        public bool IsVirtual => Attributes.HasFlag(MethodAttributes.Virtual);

        public MethodBody MethodBody => _methodBody.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.ImplAttributes" />
        public MethodImplAttributes MethodImplementationFlags { get; }

        public override string Name { get; }

        public override ImmutableArray<IParameter> Parameters => _parameters.Value;

        protected MethodSignature<TypeBase> Signature => _signature.Value;

        public Document Document => DebugInformation?.Document;

        public string SourceCode => DebugInformation?.SourceCode;

        private ImmutableArray<IParameter> LoadParameters()
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
                foreach (var parameter in parameters)
                {
                    parameter.ParameterType = Signature.ParameterTypes[parameter.Position];
                    parameter.Member = this;
                }
            }

            return parameters.Cast<IParameter>().ToImmutableArray();
        }
    }
}
