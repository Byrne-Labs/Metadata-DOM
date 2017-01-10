using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.LocalConstant" />
    //[PublicAPI]
    public class LocalConstant : DebugCodeElement, ICodeElementWithHandle<LocalConstantHandle, System.Reflection.Metadata.LocalConstant>
    {
        private readonly Lazy<Blob> _signature;

        internal LocalConstant(LocalConstantHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetLocalConstant(metadataHandle);
            Name = AsString(MetadataToken.Name);
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.Signature)));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.LocalConstant.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.LocalConstant.Signature" />
        public Blob Signature => _signature.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public LocalConstantHandle MetadataHandle { get; }

        public System.Reflection.Metadata.LocalConstant MetadataToken { get; }
    }
}
