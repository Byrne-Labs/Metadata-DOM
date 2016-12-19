using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodDebugInformation" />
    //[PublicAPI]
    public class MethodDebugInformation : DebugCodeElement, ICodeElementWithHandle<MethodDebugInformationHandle, System.Reflection.Metadata.MethodDebugInformation>, IContainsSourceCode
    {
        private readonly Lazy<Document> _document;
        private readonly Lazy<StandaloneSignature> _localSignature;
        private readonly Lazy<IEnumerable<SequencePoint>> _sequencePoints;
        private readonly Lazy<Blob> _sequencePointsBlob;
        private readonly Lazy<string> _sourceCode;
        private readonly Lazy<MethodDefinition> _stateMachineKickoffMethod;

        internal MethodDebugInformation(MethodDebugInformationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetMethodDebugInformation(metadataHandle);
            _document = MetadataState.GetLazyCodeElement<Document>(MetadataToken.Document);
            _localSignature = MetadataState.GetLazyCodeElement<StandaloneSignature>(MetadataToken.LocalSignature);
            _sequencePointsBlob = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.SequencePointsBlob)));
            _sequencePoints = MetadataState.GetLazyCodeElements<SequencePoint>(MetadataToken.GetSequencePoints());
            _stateMachineKickoffMethod = MetadataState.GetLazyCodeElement<MethodDefinition>(MetadataToken.GetStateMachineKickoffMethod());
            _sourceCode = new Lazy<string>(() => Document == null ? null : string.Join(Environment.NewLine, SequencePoints.Where(sequencePoint => sequencePoint != null).Select(sequencePoint => sequencePoint.SourceCode)));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDebugInformation.LocalSignature" />
        public StandaloneSignature LocalSignature => _localSignature.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDebugInformation.GetSequencePoints" />
        public IEnumerable<SequencePoint> SequencePoints => _sequencePoints.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDebugInformation.SequencePointsBlob" />
        /// <summary></summary>
        public Blob SequencePointsBlob => _sequencePointsBlob.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDebugInformation.GetStateMachineKickoffMethod" />
        public MethodDefinition StateMachineKickoffMethod => _stateMachineKickoffMethod.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public MethodDebugInformationHandle MetadataHandle { get; }

        public System.Reflection.Metadata.MethodDebugInformation MetadataToken { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDebugInformation.Document" />
        public Document Document => _document.Value;

        public string SourceCode => _sourceCode.Value;
    }
}
