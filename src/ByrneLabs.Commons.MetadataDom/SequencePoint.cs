using System;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.SequencePoint" />
    [PublicAPI]
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

        /// <inheritdoc cref="System.Reflection.Metadata.SequencePoint.Document" />
        public Document Document => _document.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.SequencePoint.EndColumn" />
        public int EndColumn { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.SequencePoint.EndLine" />
        public int EndLine { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.SequencePoint.IsHidden" />
        public bool IsHidden { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.SequencePoint.Offset" />
        public int Offset { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.SequencePoint.StartColumn" />
        public int StartColumn { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.SequencePoint.StartLine" />
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
