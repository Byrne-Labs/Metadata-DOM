using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;
using System.Collections.Immutable;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.StandaloneSignature" />
    //[PublicAPI]
    public class StandaloneSignature : RuntimeCodeElement, ICodeElementWithHandle<StandaloneSignatureHandle, System.Reflection.Metadata.StandaloneSignature>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<ImmutableArray<TypeBase>> _localSignature;
        private readonly Lazy<MethodSignature<TypeBase>> _methodSignature;

        internal StandaloneSignature(StandaloneSignatureHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetStandaloneSignature(metadataHandle);
            _localSignature = new Lazy<ImmutableArray<TypeBase>>(() => MetadataToken.DecodeLocalSignature(MetadataState.TypeProvider, GenericContext));
            _methodSignature = new Lazy<MethodSignature<TypeBase>>(() => MetadataToken.DecodeMethodSignature(MetadataState.TypeProvider, GenericContext));
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
            Kind = MetadataToken.GetKind();
        }

        /// <inheritdoc cref="System.Reflection.Metadata.StandaloneSignature.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.StandaloneSignature.GetKind" />
        /// <summary>Determines the kind of signature, which can be <see cref="System.Reflection.Metadata.SignatureKind.Method" /> or
        ///     <see cref="System.Reflection.Metadata.SignatureKind.LocalVariables" />
        /// </summary>
        /// <exception cref="System.BadImageFormatException">The signature is invalid.</exception>
        public StandaloneSignatureKind Kind { get; }

        internal GenericContext GenericContext { get; set; }

        public Handle DowncastMetadataHandle => MetadataHandle;

        public StandaloneSignatureHandle MetadataHandle { get; }

        public System.Reflection.Metadata.StandaloneSignature MetadataToken { get; }
    }
}
