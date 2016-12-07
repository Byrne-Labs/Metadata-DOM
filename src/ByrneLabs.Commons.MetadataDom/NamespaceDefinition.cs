using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
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

        public IEnumerable<ExportedType> ExportedTypes => _exportedTypes.Value;

        public string Name => _name.Value;

        public IEnumerable<NamespaceDefinition> NamespaceDefinitions => _namespaceDefinitions.Value;

        public NamespaceDefinition Parent => _parent.Value;

        public IEnumerable<TypeDefinition> TypeDefinitions => _typeDefinitions.Value;
    }
}
