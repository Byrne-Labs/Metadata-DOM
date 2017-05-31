using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using MethodBaseToExpose = System.Reflection.MethodBase;

#else
using MethodBaseToExpose = ByrneLabs.Commons.MetadataDom.MethodBase;

#endif
namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class MethodDebugInformation : IManagedCodeElement
    {
        private readonly Lazy<Document> _document;
        private readonly Lazy<StandaloneSignature> _localSignature;
        private readonly Lazy<ImmutableArray<SequencePoint>> _sequencePoints;
        private readonly Lazy<byte[]> _sequencePointsBlob;
        private readonly Lazy<string> _sourceCode;
        private readonly Lazy<MethodDefinition> _stateMachineKickoffMethod;

        internal MethodDebugInformation(MethodDebugInformationHandle metadataHandle, MethodBaseToExpose methodBase, GenericContext genericContext, MetadataState metadataState)
        {
            Key = new CodeElementKey<MethodDebugInformation>(metadataHandle);
            MetadataState = metadataState;
            RawMetadata = MetadataState.PdbReader.GetMethodDebugInformation(metadataHandle);
            Method = methodBase;
            _document = MetadataState.GetLazyCodeElement<Document>(RawMetadata.Document);
            _localSignature = MetadataState.GetLazyCodeElement<StandaloneSignature>(RawMetadata.LocalSignature, genericContext);
            _sequencePointsBlob = new Lazy<byte[]>(() => MetadataState.PdbReader.GetBlobBytes(RawMetadata.SequencePointsBlob));
            _sequencePoints = MetadataState.GetLazyCodeElements<SequencePoint>(RawMetadata.GetSequencePoints());
            _stateMachineKickoffMethod = MetadataState.GetLazyCodeElement<MethodDefinition>(RawMetadata.GetStateMachineKickoffMethod());
            _sourceCode = new Lazy<string>(() => Document == null ? null : string.Join(Environment.NewLine, SequencePoints.Where(sequencePoint => sequencePoint != null).Select(sequencePoint => sequencePoint.SourceCode)));
        }

        public Document Document => _document.Value;

        public string FullName => throw new NotSupportedException();

        public StandaloneSignature LocalSignature => _localSignature.Value;

        public MethodBaseToExpose Method { get; }

        public string Name => throw new NotSupportedException();

        public System.Reflection.Metadata.MethodDebugInformation RawMetadata { get; }

        public ImmutableArray<SequencePoint> SequencePoints => _sequencePoints.Value;

        public byte[] SequencePointsBlob => _sequencePointsBlob.Value;

        public string SourceCode => _sourceCode.Value;

        public MethodDefinition StateMachineKickoffMethod => _stateMachineKickoffMethod.Value;

        public string TextSignature => throw new NotSupportedException();

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
