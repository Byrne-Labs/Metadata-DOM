using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public class NamespaceDefinition : IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<ExportedType>> _exportedTypes;
        private readonly Lazy<ImmutableArray<NamespaceDefinition>> _namespaceDefinitions;
        private readonly Lazy<NamespaceDefinition> _parent;
        private readonly Lazy<ImmutableArray<TypeDefinition>> _typeDefinitions;

        internal NamespaceDefinition(NamespaceDefinitionHandle metadataHandle, MetadataState metadataState)
        {
            Key = new CodeElementKey(metadataHandle);
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.AssemblyReader.GetNamespaceDefinition(metadataHandle);
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            _parent = MetadataState.GetLazyCodeElement<NamespaceDefinition>(RawMetadata.Parent);
            _namespaceDefinitions = MetadataState.GetLazyCodeElements<NamespaceDefinition>(RawMetadata.NamespaceDefinitions);
            _typeDefinitions = MetadataState.GetLazyCodeElements<TypeDefinition>(RawMetadata.TypeDefinitions);
            _exportedTypes = MetadataState.GetLazyCodeElements<ExportedType>(RawMetadata.ExportedTypes);
        }

        public ImmutableArray<ExportedType> ExportedTypes => _exportedTypes.Value;

        public NamespaceDefinitionHandle MetadataHandle { get; }

        public ImmutableArray<NamespaceDefinition> NamespaceDefinitions => _namespaceDefinitions.Value;

        public NamespaceDefinition Parent => _parent.Value;

        public System.Reflection.Metadata.NamespaceDefinition RawMetadata { get; }

        public ImmutableArray<TypeDefinition> TypeDefinitions => _typeDefinitions.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        public string FullName => Name;

        public string Name { get; }

        public string TextSignature => Name;

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
