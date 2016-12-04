using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class CustomAttribute : CodeElementWithHandle
    {
        private readonly Lazy<CodeElement> _constructor;
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<Blob> _value;

        internal CustomAttribute(CustomAttributeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var customAttribute = Reader.GetCustomAttribute(metadataHandle);
            _constructor = new Lazy<CodeElement>(() => GetCodeElement(customAttribute.Constructor));
            _parent = new Lazy<CodeElement>(() => GetCodeElement(customAttribute.Parent));
            _value = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(customAttribute.Value)));
        }

        public CodeElement Constructor => _constructor.Value;

        public CodeElement Parent => _parent.Value;

        public Blob Value => _value.Value;
    }
}
