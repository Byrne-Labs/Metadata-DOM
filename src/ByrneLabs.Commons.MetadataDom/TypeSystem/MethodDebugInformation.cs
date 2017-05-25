using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class MethodDebugInformation : IManagedCodeElement
    {
        private readonly Lazy<Document> _document;
        private readonly Lazy<StandaloneSignature> _localSignature;
        private readonly Lazy<ImmutableArray<SequencePoint>> _sequencePoints;
        private readonly Lazy<byte[]> _sequencePointsBlob;
        private readonly Lazy<string> _sourceCode;
        private readonly Lazy<MethodDefinition> _stateMachineKickoffMethod;

        internal MethodDebugInformation(MethodDebugInformationHandle metadataHandle, MetadataState metadataState)
        {
            Key = new CodeElementKey(metadataHandle);
            MetadataState = metadataState;
            RawMetadata = MetadataState.PdbReader.GetMethodDebugInformation(metadataHandle);
            _document = MetadataState.GetLazyCodeElement<Document>(RawMetadata.Document);
            _localSignature = MetadataState.GetLazyCodeElement<StandaloneSignature>(RawMetadata.LocalSignature);
            _sequencePointsBlob = new Lazy<byte[]>(() => MetadataState.AssemblyReader.GetBlobBytes(RawMetadata.SequencePointsBlob));
            _sequencePoints = MetadataState.GetLazyCodeElements<SequencePoint>(RawMetadata.GetSequencePoints());
            _stateMachineKickoffMethod = MetadataState.GetLazyCodeElement<MethodDefinition>(RawMetadata.GetStateMachineKickoffMethod());
            _sourceCode = new Lazy<string>(() => Document == null ? null : string.Join(Environment.NewLine, SequencePoints.Where(sequencePoint => sequencePoint != null).Select(sequencePoint => sequencePoint.SourceCode)));
        }

        public Document Document => _document.Value;

        public StandaloneSignature LocalSignature => _localSignature.Value;

        public System.Reflection.Metadata.MethodDebugInformation RawMetadata { get; }

        public ImmutableArray<SequencePoint> SequencePoints => _sequencePoints.Value;

        public byte[] SequencePointsBlob => _sequencePointsBlob.Value;

        public string SourceCode => _sourceCode.Value;

        public MethodDefinition StateMachineKickoffMethod => _stateMachineKickoffMethod.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public string FullName => throw new NotSupportedException();

        public string Name => throw new NotSupportedException();

        public string TextSignature => throw new NotSupportedException();
    }
}
