using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class StandaloneSignature : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<Blob> _signature;

        internal StandaloneSignature(StandaloneSignatureHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var standaloneSignature = Reader.GetStandaloneSignature(metadataHandle);
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(standaloneSignature.Signature)));
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(standaloneSignature.GetCustomAttributes()));
            Kind = standaloneSignature.GetKind();
        }

        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public StandaloneSignatureKind Kind { get; }

        public Blob Signature => _signature.Value;
    }
}
