using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodDebugInformation" />
    public class MethodDebugInformation : DebugCodeElementWithHandle, IContainsSourceCode
    {
        private readonly Lazy<Document> _document;
        private readonly Lazy<StandaloneSignature> _localSignature;
        private readonly Lazy<IReadOnlyList<SequencePoint>> _sequencePoints;
        private readonly Lazy<Blob> _sequencePointsBlob;
        private readonly Lazy<string> _sourceCode;
        private readonly Lazy<MethodDefinition> _stateMachineKickoffMethod;

        internal MethodDebugInformation(MethodDebugInformationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var methodDebugInformation = Reader.GetMethodDebugInformation(metadataHandle);
            _document = new Lazy<Document>(() => GetCodeElement<Document>(methodDebugInformation.Document));
            _localSignature = new Lazy<StandaloneSignature>(() => GetCodeElement<StandaloneSignature>(methodDebugInformation.LocalSignature));
            _sequencePointsBlob = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(methodDebugInformation.SequencePointsBlob)));
            _sequencePoints = new Lazy<IReadOnlyList<SequencePoint>>(() => GetCodeElements<SequencePoint>(methodDebugInformation.GetSequencePoints().Select(sequencePoint => new HandlelessCodeElementKey<SequencePoint>(sequencePoint))));
            _stateMachineKickoffMethod = new Lazy<MethodDefinition>(() => GetCodeElement<MethodDefinition>(methodDebugInformation.GetStateMachineKickoffMethod()));
            _sourceCode = new Lazy<string>(() => Document == null ? null : string.Join(Environment.NewLine, SequencePoints.Select(sequencePoint => sequencePoint.SourceCode)));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDebugInformation.Document" />
        public Document Document => _document.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDebugInformation.LocalSignature" />
        public StandaloneSignature LocalSignature => _localSignature.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDebugInformation.GetSequencePoints" />
        public IReadOnlyList<SequencePoint> SequencePoints => _sequencePoints.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDebugInformation.SequencePointsBlob" />
        /// <summary></summary>
        public Blob SequencePointsBlob => _sequencePointsBlob.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDebugInformation.GetStateMachineKickoffMethod" />
        public MethodDefinition StateMachineKickoffMethod => _stateMachineKickoffMethod.Value;

        public string SourceCode => _sourceCode.Value;

        public string SourceFile => Document?.SourceFile;
    }
}
