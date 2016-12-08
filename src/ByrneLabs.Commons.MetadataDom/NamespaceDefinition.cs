using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition" />
    [PublicAPI]
    public class NamespaceDefinition : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<ExportedType>> _exportedTypes;
        private readonly Lazy<string> _name;
        private readonly Lazy<IEnumerable<NamespaceDefinition>> _namespaceDefinitions;
        private readonly Lazy<NamespaceDefinition> _parent;
        private readonly Lazy<IEnumerable<TypeDefinition>> _typeDefinitions;

        internal NamespaceDefinition(NamespaceDefinitionHandle namespaceDefinitionHandle, MetadataState metadataState) : base(namespaceDefinitionHandle, metadataState)
        {
            var namespaceDefinition = Reader.GetNamespaceDefinition(namespaceDefinitionHandle);
            _name = new Lazy<string>(() => AsString(namespaceDefinition.Name));
            _parent = new Lazy<NamespaceDefinition>(() => GetCodeElement<NamespaceDefinition>(namespaceDefinition.Parent));
            _namespaceDefinitions = new Lazy<IEnumerable<NamespaceDefinition>>(() => GetCodeElements<NamespaceDefinition>(namespaceDefinition.NamespaceDefinitions));
            _typeDefinitions = new Lazy<IEnumerable<TypeDefinition>>(() => GetCodeElements<TypeDefinition>(namespaceDefinition.TypeDefinitions));
            _exportedTypes = new Lazy<IEnumerable<ExportedType>>(() => GetCodeElements<ExportedType>(namespaceDefinition.ExportedTypes));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.ExportedTypes" />
        public IEnumerable<ExportedType> ExportedTypes => _exportedTypes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.Name" />
        public string Name => _name.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.NamespaceDefinitions" />
        public IEnumerable<NamespaceDefinition> NamespaceDefinitions => _namespaceDefinitions.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.Parent" />
        public NamespaceDefinition Parent => _parent.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.TypeDefinitions" />
        public IEnumerable<TypeDefinition> TypeDefinitions => _typeDefinitions.Value;
    }
}
