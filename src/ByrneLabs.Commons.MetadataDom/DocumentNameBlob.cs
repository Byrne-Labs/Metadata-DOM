using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class DocumentNameBlob : CodeElementWithHandle
    {
        internal DocumentNameBlob(DocumentNameBlobHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            Bytes = Reader.GetBlobBytes(metadataHandle);
            StringValue = BitConverter.ToString(Bytes);
        }

        public byte[] Bytes { get; }

        public string StringValue { get; }
    }
}
