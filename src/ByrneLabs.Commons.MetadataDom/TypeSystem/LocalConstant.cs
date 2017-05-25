using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class LocalConstant : DebugCodeElement, ICodeElementWithTypedHandle<LocalConstantHandle, System.Reflection.Metadata.LocalConstant>
    {
        private readonly Lazy<Blob> _signature;

        internal LocalConstant(LocalConstantHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetLocalConstant(metadataHandle);
            Name = AsString(RawMetadata.Name);
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(RawMetadata.Signature)));
        }
        public string Name { get; }
        public Blob Signature => _signature.Value;

        public System.Reflection.Metadata.LocalConstant RawMetadata { get; }

        public LocalConstantHandle MetadataHandle { get; }
    }
}
