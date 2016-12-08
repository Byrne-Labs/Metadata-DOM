using System;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute" />
    [PublicAPI]
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

        /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute.Constructor" />
        /// <summary>The constructor (<see cref="T:ByrneLabs.Commons.MetadataDom.MethodDefinition" /> or <see cref="T:ByrneLabs.Commons.MetadataDom.MemberReference" />) of the custom attribute type.</summary>
        public CodeElement Constructor => _constructor.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute.Parent" />
        public CodeElement Parent => _parent.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute.Value" />
        public Blob Value => _value.Value;
    }
}
