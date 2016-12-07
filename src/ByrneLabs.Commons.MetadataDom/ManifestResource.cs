using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class ManifestResource : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _implementation;
        private readonly Lazy<string> _name;

        internal ManifestResource(ManifestResourceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var manifestResource = Reader.GetManifestResource(metadataHandle);
            _name = new Lazy<string>(() => AsString(manifestResource.Name));
            Attributes = manifestResource.Attributes;
            _implementation = new Lazy<CodeElement>(() => GetCodeElement(manifestResource.Implementation));
            Offset = manifestResource.Offset;
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(manifestResource.GetCustomAttributes()));
        }

        public ManifestResourceAttributes Attributes { get; }

        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public CodeElement Implementation => _implementation.Value;

        public string Name => _name.Value;

        public long Offset { get; }
    }
}
