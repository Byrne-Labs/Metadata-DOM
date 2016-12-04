using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class NamespaceDefinition : CodeElementWithHandle
    {
        private readonly Lazy<IReadOnlyList<ExportedType>> _exportedTypes;
        private readonly Lazy<string> _name;
        private readonly Lazy<IReadOnlyList<NamespaceDefinition>> _namespaceDefinitions;
        private readonly Lazy<NamespaceDefinition> _parent;
        private readonly Lazy<IReadOnlyList<TypeDefinition>> _typeDefinitions;

        internal NamespaceDefinition(NamespaceDefinitionHandle namespaceDefinitionHandle, MetadataState metadataState) : base(namespaceDefinitionHandle, metadataState)
        {
            var namespaceDefinition = Reader.GetNamespaceDefinition(namespaceDefinitionHandle);
            _name = new Lazy<string>(() => AsString(namespaceDefinition.Name));
            _parent = new Lazy<NamespaceDefinition>(() => GetCodeElement<NamespaceDefinition>(namespaceDefinition.Parent));
            _namespaceDefinitions = new Lazy<IReadOnlyList<NamespaceDefinition>>(() => GetCodeElements<NamespaceDefinition>(namespaceDefinition.NamespaceDefinitions));
            _typeDefinitions = new Lazy<IReadOnlyList<TypeDefinition>>(() => GetCodeElements<TypeDefinition>(namespaceDefinition.TypeDefinitions));
            _exportedTypes = new Lazy<IReadOnlyList<ExportedType>>(() => GetCodeElements<ExportedType>(namespaceDefinition.ExportedTypes));
        }

        public IReadOnlyList<ExportedType> ExportedTypes => _exportedTypes.Value;

        public string Name => _name.Value;

        public IReadOnlyList<NamespaceDefinition> NamespaceDefinitions => _namespaceDefinitions.Value;

        public NamespaceDefinition Parent => _parent.Value;

        public IReadOnlyList<TypeDefinition> TypeDefinitions => _typeDefinitions.Value;
    }
}
