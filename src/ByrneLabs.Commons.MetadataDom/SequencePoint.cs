using System;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class SequencePoint : CodeElementWithoutHandle
    {
        private readonly Lazy<Document> _document;

        internal SequencePoint(System.Reflection.Metadata.SequencePoint sequencePoint, MetadataState metadataState) : base(sequencePoint, metadataState)
        {
            _document = new Lazy<Document>(() => GetCodeElement<Document>(sequencePoint.Document));
            EndColumn = sequencePoint.EndColumn;
            EndLine = sequencePoint.EndLine;
            IsHidden = sequencePoint.IsHidden;
            Offset = sequencePoint.Offset;
            StartColumn = sequencePoint.StartColumn;
            StartLine = sequencePoint.StartLine;
        }

        public Document Document => _document.Value;

        public int EndColumn { get; }

        public int EndLine { get; }

        public bool IsHidden { get; }

        public int Offset { get; }

        public int StartColumn { get; }

        public int StartLine { get; }
    }
}
