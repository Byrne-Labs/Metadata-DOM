using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.LocalConstant" />
    public class LocalConstant : DebugCodeElementWithHandle
    {
        private readonly Lazy<string> _name;
        private readonly Lazy<Blob> _signature;

        internal LocalConstant(LocalConstantHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var localConstant = Reader.GetLocalConstant(metadataHandle);
            _name = new Lazy<string>(() => AsString(localConstant.Name));
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(localConstant.Signature)));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.LocalConstant.Name" />
        public string Name => _name.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.LocalConstant.Signature" />
        public Blob Signature => _signature.Value;
    }
}
