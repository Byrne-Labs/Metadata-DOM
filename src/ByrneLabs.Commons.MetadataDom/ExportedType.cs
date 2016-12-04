using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class ExportedType : CodeElementWithHandle
    {
        private readonly Lazy<IReadOnlyList<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _implementation;
        private readonly Lazy<string> _name;
        private readonly Lazy<string> _namespace;
        private readonly Lazy<NamespaceDefinition> _namespaceDefinition;

        internal ExportedType(ExportedTypeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var exportedType = Reader.GetExportedType(metadataHandle);
            _name = new Lazy<string>(() => AsString(exportedType.Name));
            Attributes = exportedType.Attributes;
            _implementation = new Lazy<CodeElement>(() => GetCodeElement(exportedType.Implementation));
            IsForwarder = exportedType.IsForwarder;
            _namespace = new Lazy<string>(() => AsString(exportedType.Namespace));
            _namespaceDefinition = new Lazy<NamespaceDefinition>(() => GetCodeElement<NamespaceDefinition>(exportedType.NamespaceDefinition));
            _customAttributes = new Lazy<IReadOnlyList<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(exportedType.GetCustomAttributes()));
        }

        public TypeAttributes Attributes { get; }

        public IReadOnlyList<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public CodeElement Implementation => _implementation.Value;

        public bool IsForwarder { get; }

        public string Name => _name.Value;

        public string Namespace => _namespace.Value;

        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;
    }
}
