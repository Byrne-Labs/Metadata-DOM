using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name.FullName}")]
    public class AssemblyDefinition : AssemblyBase<AssemblyDefinition, AssemblyDefinitionHandle, System.Reflection.Metadata.AssemblyDefinition>
    {
        private readonly Lazy<ImmutableArray<AssemblyReference>> _assemblyReferences;
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<ImmutableArray<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<MethodDefinition> _entryPoint;
        private readonly Lazy<ModuleDefinition> _moduleDefinition;

        internal AssemblyDefinition(AssemblyDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var name = new AssemblyName
            {
                Name = AsString(RawMetadata.Name),
                CultureName = AsString(RawMetadata.Culture),
                Flags = (RawMetadata.Flags.HasFlag(AssemblyFlags.PublicKey) ? AssemblyNameFlags.PublicKey : 0) | (RawMetadata.Flags.HasFlag(AssemblyFlags.Retargetable) ? AssemblyNameFlags.Retargetable : 0),
                ContentType = RawMetadata.Flags.HasFlag(AssemblyFlags.WindowsRuntime) ? AssemblyContentType.WindowsRuntime : AssemblyContentType.Default,
                Version = RawMetadata.Version
            };
            name.SetPublicKey(Reader.GetBlobBytes(RawMetadata.PublicKey));
            Name = name;

            _entryPoint = new Lazy<MethodDefinition>(() => MetadataState.HasDebugMetadata ? MetadataState.GetCodeElement<MethodDefinition>(MetadataState.PdbReader.DebugMetadataHeader.EntryPoint) : null);
            _assemblyReferences = MetadataState.GetLazyCodeElements<AssemblyReference>(Reader.AssemblyReferences);
            Flags = RawMetadata.Flags;
            HashAlgorithm = RawMetadata.HashAlgorithm;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(RawMetadata.GetDeclarativeSecurityAttributes());
            _moduleDefinition = MetadataState.GetLazyCodeElement<ModuleDefinition>(Handle.ModuleDefinition);
        }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.GetCustomAttributes" />
        public override ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.GetDeclarativeSecurityAttributes" />
        public ImmutableArray<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public ImmutableArray<TypeBase> DefinedTypes => MetadataState.DefinedTypes;

        public IMethod EntryPoint => _entryPoint.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.Flags" />
        public override AssemblyFlags Flags { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.HashAlgorithm" />
        public AssemblyHashAlgorithm HashAlgorithm { get; }

        public IModule Module => _moduleDefinition.Value;

        public override AssemblyName Name { get; }

        public ImmutableArray<AssemblyReference> ReferencedAssemblies => _assemblyReferences.Value;
    }
}
