using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class MethodDebugInformation : IManagedCodeElement
    {
        private readonly Lazy<Document> _document;
        private readonly Lazy<StandaloneSignature> _localSignature;
        private readonly Lazy<IEnumerable<SequencePoint>> _sequencePoints;
        private readonly Lazy<Blob> _sequencePointsBlob;
        private readonly Lazy<string> _sourceCode;
        private readonly Lazy<MethodDefinition> _stateMachineKickoffMethod;

        internal MethodDebugInformation(MethodDebugInformationHandle metadataHandle, MetadataState metadataState) : this(metadataHandle, null, null, metadataState)
        {
        }

        internal MethodDebugInformation(MethodDebugInformationHandle metadataHandle, MethodBase methodBase, GenericContext genericContext, MetadataState metadataState)
        {
            Key = new CodeElementKey<MethodDebugInformation>(metadataHandle);
            MetadataState = metadataState;
            RawMetadata = MetadataState.PdbReader.GetMethodDebugInformation(metadataHandle);
            Method = methodBase;
            _document = MetadataState.GetLazyCodeElement<Document>(RawMetadata.Document);
            _localSignature = MetadataState.GetLazyCodeElement<StandaloneSignature>(RawMetadata.LocalSignature, genericContext);
            _sequencePointsBlob = MetadataState.GetLazyCodeElement<Blob>(RawMetadata.SequencePointsBlob, true);
            _sequencePoints = MetadataState.GetLazyCodeElements<SequencePoint>(RawMetadata.GetSequencePoints());
            _stateMachineKickoffMethod = MetadataState.GetLazyCodeElement<MethodDefinition>(RawMetadata.GetStateMachineKickoffMethod());
            _sourceCode = new Lazy<string>(() => Document == null ? null : string.Join(Environment.NewLine, SequencePoints.Where(sequencePoint => sequencePoint != null).Select(sequencePoint => sequencePoint.SourceCode)));
        }

        public Document Document => _document.Value;

        public StandaloneSignature LocalSignature => _localSignature.Value;

        public MethodBase Method { get; }

        public System.Reflection.Metadata.MethodDebugInformation RawMetadata { get; }

        public IEnumerable<SequencePoint> SequencePoints => _sequencePoints.Value;

        public Blob SequencePointsBlob => _sequencePointsBlob.Value;

        public string SourceCode => _sourceCode.Value;

        public MethodDefinition StateMachineKickoffMethod => _stateMachineKickoffMethod.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
