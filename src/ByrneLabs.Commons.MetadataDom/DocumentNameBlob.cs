using System;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
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
