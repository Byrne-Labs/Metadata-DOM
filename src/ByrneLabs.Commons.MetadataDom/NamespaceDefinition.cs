using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public class NamespaceDefinition : RuntimeCodeElement, ICodeElementWithTypedHandle<NamespaceDefinitionHandle, System.Reflection.Metadata.NamespaceDefinition>
    {
        private readonly Lazy<ImmutableArray<ExportedType>> _exportedTypes;
        private readonly Lazy<ImmutableArray<NamespaceDefinition>> _namespaceDefinitions;
        private readonly Lazy<NamespaceDefinition> _parent;
        private readonly Lazy<ImmutableArray<TypeDefinition>> _typeDefinitions;

        internal NamespaceDefinition(NamespaceDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetNamespaceDefinition(metadataHandle);
            Name = AsString(RawMetadata.Name);
            _parent = MetadataState.GetLazyCodeElement<NamespaceDefinition>(RawMetadata.Parent);
            _namespaceDefinitions = MetadataState.GetLazyCodeElements<NamespaceDefinition>(RawMetadata.NamespaceDefinitions);
            _typeDefinitions = MetadataState.GetLazyCodeElements<TypeDefinition>(RawMetadata.TypeDefinitions);
            _exportedTypes = MetadataState.GetLazyCodeElements<ExportedType>(RawMetadata.ExportedTypes);
        }

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.ExportedTypes" />
        public ImmutableArray<ExportedType> ExportedTypes => _exportedTypes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.NamespaceDefinitions" />
        public ImmutableArray<NamespaceDefinition> NamespaceDefinitions => _namespaceDefinitions.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.Parent" />
        public NamespaceDefinition Parent => _parent.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.TypeDefinitions" />
        public ImmutableArray<TypeDefinition> TypeDefinitions => _typeDefinitions.Value;

        public System.Reflection.Metadata.NamespaceDefinition RawMetadata { get; }

        public NamespaceDefinitionHandle MetadataHandle { get; }
    }
}
