using System;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute" />
    [PublicAPI]
    public class CustomAttribute : RuntimeCodeElement, ICodeElementWithHandle<CustomAttributeHandle, System.Reflection.Metadata.CustomAttribute>
    {
        private readonly Lazy<CodeElement> _constructor;
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<Blob> _value;

        internal CustomAttribute(CustomAttributeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetCustomAttribute(metadataHandle);
            _constructor = MetadataState.GetLazyCodeElement(MetadataToken.Constructor);
            _parent = MetadataState.GetLazyCodeElement(MetadataToken.Parent);
            _value = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.Value)));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute.Constructor" />
        /// <summary>The constructor (<see cref="MethodDefinition" /> or <see cref="MemberReference" />) of the custom attribute type.</summary>
        public CodeElement Constructor => _constructor.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute.Parent" />
        public CodeElement Parent => _parent.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.CustomAttribute.Value" />
        public Blob Value => _value.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public CustomAttributeHandle MetadataHandle { get; }

        public System.Reflection.Metadata.CustomAttribute MetadataToken { get; }
    }
}
