using System;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class Document : DebugCodeElementWithHandle
    {
        private readonly Lazy<Blob> _hash;
        private readonly Lazy<Blob> _name;

        internal Document(DocumentHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var document = Reader.GetDocument(metadataHandle);
            _name = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(document.Name)));
            _hash = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(document.Hash)));
            HashAlgorithm = AsGuid(document.HashAlgorithm);
            Language = AsGuid(document.Language);
        }

        public Blob Hash => _hash.Value;

        public Guid HashAlgorithm { get; }

        public Guid Language { get; }

        public Blob Name => _name.Value;
    }
}
