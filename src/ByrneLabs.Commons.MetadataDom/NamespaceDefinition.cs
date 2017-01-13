using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public class NamespaceDefinition : RuntimeCodeElement, ICodeElementWithTypedHandle<NamespaceDefinitionHandle, System.Reflection.Metadata.NamespaceDefinition>
    {
        private readonly Lazy<IEnumerable<ExportedType>> _exportedTypes;
        private readonly Lazy<IEnumerable<NamespaceDefinition>> _namespaceDefinitions;
        private readonly Lazy<NamespaceDefinition> _parent;
        private readonly Lazy<IEnumerable<TypeDefinition>> _typeDefinitions;

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
        public IEnumerable<ExportedType> ExportedTypes => _exportedTypes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.NamespaceDefinitions" />
        public IEnumerable<NamespaceDefinition> NamespaceDefinitions => _namespaceDefinitions.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.Parent" />
        public NamespaceDefinition Parent => _parent.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.TypeDefinitions" />
        public IEnumerable<TypeDefinition> TypeDefinitions => _typeDefinitions.Value;

        public System.Reflection.Metadata.NamespaceDefinition RawMetadata { get; }

        public NamespaceDefinitionHandle MetadataHandle { get; }
    }
}
