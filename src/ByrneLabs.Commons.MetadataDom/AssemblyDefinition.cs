using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition" />
    //[PublicAPI]
    public class AssemblyDefinition : AssemblyBase<AssemblyDefinition, AssemblyDefinitionHandle, System.Reflection.Metadata.AssemblyDefinition>
    {
        private readonly Lazy<IEnumerable<AssemblyReference>> _assemblyReferences;
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<MethodDefinition> _entryPoint;
        private readonly Lazy<ModuleDefinition> _moduleDefinition;

        internal AssemblyDefinition(AssemblyDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var name = new AssemblyName
            {
                Name = AsString(MetadataToken.Name),
                CultureName = AsString(MetadataToken.Culture),
                Flags = (MetadataToken.Flags.HasFlag(AssemblyFlags.PublicKey) ? AssemblyNameFlags.PublicKey : 0) | (MetadataToken.Flags.HasFlag(AssemblyFlags.Retargetable) ? AssemblyNameFlags.Retargetable : 0),
                ContentType = MetadataToken.Flags.HasFlag(AssemblyFlags.WindowsRuntime) ? AssemblyContentType.WindowsRuntime : AssemblyContentType.Default,
                Version = MetadataToken.Version
            };
            name.SetPublicKey(Reader.GetBlobBytes(MetadataToken.PublicKey));
            Name = name;

            _entryPoint = new Lazy<MethodDefinition>(() => MetadataState.HasDebugMetadata ? MetadataState.GetCodeElement<MethodDefinition>(MetadataState.PdbReader.DebugMetadataHeader.EntryPoint) : null);
            _assemblyReferences = MetadataState.GetLazyCodeElements<AssemblyReference>(Reader.AssemblyReferences);
            Flags = MetadataToken.Flags;
            HashAlgorithm = MetadataToken.HashAlgorithm;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(MetadataToken.GetDeclarativeSecurityAttributes());
            _moduleDefinition = MetadataState.GetLazyCodeElement<ModuleDefinition>(Handle.ModuleDefinition);
        }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.GetCustomAttributes" />
        public override IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.GetDeclarativeSecurityAttributes" />
        public override IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public override IEnumerable<TypeBase> DefinedTypes => MetadataState.DefinedTypes;

        public override IMethod EntryPoint => _entryPoint.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.Flags" />
        public override AssemblyFlags Flags { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.HashAlgorithm" />
        public override AssemblyHashAlgorithm HashAlgorithm { get; }

        public IModule Module => _moduleDefinition.Value;

        public override AssemblyName Name { get; }

        public override IEnumerable<IAssembly> ReferencedAssemblies => _assemblyReferences.Value;
    }
}
