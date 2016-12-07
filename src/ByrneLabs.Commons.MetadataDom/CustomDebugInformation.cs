using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.CustomDebugInformation" />
    public class CustomDebugInformation : DebugCodeElementWithHandle
    {
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<Blob> _value;

        internal CustomDebugInformation(CustomDebugInformationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var customDebugInformation = Reader.GetCustomDebugInformation(metadataHandle);
            _parent = new Lazy<CodeElement>(() => GetCodeElement(customDebugInformation.Parent));
            Kind = AsGuid(customDebugInformation.Kind);
            _value = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(customDebugInformation.Value)));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.CustomDebugInformation.Kind" />
        public Guid Kind { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.CustomDebugInformation.Parent" />
        public CodeElement Parent => _parent.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.CustomDebugInformation.Value" />
        public Blob Value => _value.Value;
    }
}
