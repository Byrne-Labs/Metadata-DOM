using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.StandaloneSignature" />
    //[PublicAPI]
    public class StandaloneSignature : RuntimeCodeElement, ICodeElementWithTypedHandle<StandaloneSignatureHandle, System.Reflection.Metadata.StandaloneSignature>
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<ImmutableArray<TypeBase>> _localSignature;
        private readonly Lazy<MethodSignature<TypeBase>> _methodSignature;

        internal StandaloneSignature(StandaloneSignatureHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetStandaloneSignature(metadataHandle);
            _localSignature = new Lazy<ImmutableArray<TypeBase>>(() => RawMetadata.DecodeLocalSignature(MetadataState.TypeProvider, GenericContext));
            _methodSignature = new Lazy<MethodSignature<TypeBase>>(() => RawMetadata.DecodeMethodSignature(MetadataState.TypeProvider, GenericContext));
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            Kind = RawMetadata.GetKind();
        }

        /// <inheritdoc cref="System.Reflection.Metadata.StandaloneSignature.GetCustomAttributes" />
        public ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.StandaloneSignature.GetKind" />
        /// <summary>Determines the kind of signature, which can be <see cref="System.Reflection.Metadata.SignatureKind.Method" /> or
        ///     <see cref="System.Reflection.Metadata.SignatureKind.LocalVariables" />
        /// </summary>
        /// <exception cref="System.BadImageFormatException">The signature is invalid.</exception>
        public StandaloneSignatureKind Kind { get; }

        internal GenericContext GenericContext { get; set; }

        public System.Reflection.Metadata.StandaloneSignature RawMetadata { get; }

        public StandaloneSignatureHandle MetadataHandle { get; }
    }
}
