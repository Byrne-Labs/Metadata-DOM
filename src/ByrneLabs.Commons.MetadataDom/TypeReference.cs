using System;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class TypeReference : CodeElementWithHandle
    {
        private readonly Lazy<string> _name;
        private readonly Lazy<string> _namespace;
        private readonly Lazy<CodeElement> _resolutionScope;

        internal TypeReference(TypeReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var typeReference = Reader.GetTypeReference(metadataHandle);
            _name = new Lazy<string>(() => AsString(typeReference.Name));
            _namespace = new Lazy<string>(() => AsString(typeReference.Namespace));
            _resolutionScope = new Lazy<CodeElement>(() => GetCodeElement(typeReference.ResolutionScope));
        }

        public string Name => _name.Value;

        public string Namespace => _namespace.Value;

        public CodeElement ResolutionScope => _resolutionScope.Value;
    }
}
