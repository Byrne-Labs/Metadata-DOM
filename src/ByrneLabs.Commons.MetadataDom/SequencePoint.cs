using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ByrneLabs.Commons.MetadataDom
{
    public class SequencePoint : CodeElementWithoutHandle, IContainsSourceCode
    {
        private readonly Lazy<Document> _document;
        private readonly Lazy<string> _sourceCode;

        internal SequencePoint(System.Reflection.Metadata.SequencePoint sequencePoint, MetadataState metadataState) : base(new HandlelessCodeElementKey<SequencePoint>(sequencePoint), metadataState)
        {
            _document = new Lazy<Document>(() => GetCodeElement<Document>(sequencePoint.Document));
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

        public bool IsHidden { get; }

        public int Offset { get; }

        public int StartColumn { get; }

        public int StartLine { get; }

        public string SourceCode => _sourceCode.Value;

        public string SourceFile => Document.SourceFile;

        private string LoadSourceCode()
        {
            string sourceCode;
            if (StartLine == 16707566)
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
