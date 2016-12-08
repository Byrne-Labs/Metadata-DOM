using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.StandaloneSignature" />
    [PublicAPI]
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

        /// <inheritdoc cref="System.Reflection.Metadata.StandaloneSignature.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.StandaloneSignature.GetKind" />
        /// <summary>Determines the kind of signature, which can be <see cref="System.Reflection.Metadata.SignatureKind.Method" /> or
        ///     <see cref="System.Reflection.Metadata.SignatureKind.LocalVariables" />
        /// </summary>
        /// <exception cref="System.BadImageFormatException">The signature is invalid.</exception>
        public StandaloneSignatureKind Kind { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.StandaloneSignature.Signature" />
        /// <summary></summary>
        public Blob Signature => _signature.Value;
    }
}
