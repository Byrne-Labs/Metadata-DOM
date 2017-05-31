using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class Blob : SimpleCodeElement
    {
        internal Blob(BlobHandle metadataHandle, bool debug, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            Bytes = debug ? MetadataState.PdbReader.GetBlobBytes(metadataHandle) : MetadataState.AssemblyReader.GetBlobBytes(metadataHandle);
        }

        public byte[] Bytes { get; }

        public BlobHandle MetadataHandle { get; }
    }
}
