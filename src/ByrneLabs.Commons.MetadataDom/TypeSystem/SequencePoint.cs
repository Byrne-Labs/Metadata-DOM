using System;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class SequencePoint : IManagedCodeElement
    {
        private readonly Lazy<Document> _document;
        private readonly Lazy<string> _sourceCode;

        internal SequencePoint(System.Reflection.Metadata.SequencePoint sequencePoint, MetadataState metadataState)
        {
            Key = new CodeElementKey<SequencePoint>(sequencePoint);
            MetadataState = metadataState;
            RawMetadata = sequencePoint;
            _document = MetadataState.GetLazyCodeElement<Document>(sequencePoint.Document);
            EndColumn = sequencePoint.EndColumn;
            EndLine = sequencePoint.EndLine;
            IsHidden = sequencePoint.IsHidden;
            Offset = sequencePoint.Offset;
            StartColumn = sequencePoint.StartColumn;
            StartLine = sequencePoint.StartLine;
            _sourceCode = new Lazy<string>(LoadSourceCode);
        }

        public Document Document => _document.Value;

        public int EndColumn { get; }

        public int EndLine { get; }

        public string FullName => Name;

        public bool IsHidden { get; }

        public string Name => $"{Document.Name}({StartLine}:{StartColumn},{EndLine},{EndColumn})";

        public int Offset { get; }

        public System.Reflection.Metadata.SequencePoint RawMetadata { get; }

        public string SourceCode => _sourceCode.Value;

        public int StartColumn { get; }

        public int StartLine { get; }

        public string TextSignature => Name;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        private string LoadSourceCode()
        {
            string sourceCode;
            if (StartLine == 16707566 || Document.SourceCode == null)
            {
                sourceCode = null;
            }
            else if (StartLine == EndLine)
            {
                sourceCode = Document.SourceCodeLines[StartLine - 1].Substring(StartColumn - 1, EndColumn - StartColumn);
            }
            else
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(Document.SourceCodeLines[StartLine - 1].Substring(StartColumn - 1));
                foreach (var line in Document.SourceCodeLines.Skip(StartLine).Take(EndLine - StartLine - 1))
                {
                    stringBuilder.AppendLine(line);
                }

                stringBuilder.Append(Document.SourceCodeLines[EndLine - 1].Substring(0, EndColumn - 1));
                sourceCode = stringBuilder.ToString();
            }

            return sourceCode;
        }
    }
}
