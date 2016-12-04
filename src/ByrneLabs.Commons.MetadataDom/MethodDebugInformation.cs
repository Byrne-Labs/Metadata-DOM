﻿using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class MethodDebugInformation : DebugCodeElementWithHandle
    {
        private readonly Lazy<Document> _document;
        private readonly Lazy<StandaloneSignature> _localSignature;
        private readonly Lazy<IReadOnlyList<SequencePoint>> _sequencePoints;
        private readonly Lazy<Blob> _sequencePointsBlob;
        private readonly Lazy<MethodDefinition> _stateMachineKickoffMethod;

        internal MethodDebugInformation(MethodDebugInformationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var methodDebugInformation = Reader.GetMethodDebugInformation(metadataHandle);
            _document = new Lazy<Document>(() => GetCodeElement<Document>(methodDebugInformation.Document));
            _localSignature = new Lazy<StandaloneSignature>(() => GetCodeElement<StandaloneSignature>(methodDebugInformation.LocalSignature));
            _sequencePointsBlob = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(methodDebugInformation.SequencePointsBlob)));
            _sequencePoints = new Lazy<IReadOnlyList<SequencePoint>>(() => GetCodeElements<SequencePoint>(methodDebugInformation.GetSequencePoints()));
            _stateMachineKickoffMethod = new Lazy<MethodDefinition>(() => GetCodeElement<MethodDefinition>(methodDebugInformation.GetStateMachineKickoffMethod()));
        }

        public Document Document => _document.Value;

        public StandaloneSignature LocalSignature => _localSignature.Value;

        public IReadOnlyList<SequencePoint> SequencePoints => _sequencePoints.Value;

        public Blob SequencePointsBlob => _sequencePointsBlob.Value;

        public MethodDefinition StateMachineKickoffMethod => _stateMachineKickoffMethod.Value;
    }
}
