using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
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

        public string Name => _name.Value;

        public Blob Signature => _signature.Value;
    }
}
