using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ExceptionRegion" />
    //[PublicAPI]
    public class ExceptionRegion : RuntimeCodeElement, ICodeElementWithRawMetadata<System.Reflection.Metadata.ExceptionRegion>
    {
        private readonly Lazy<TypeBase> _catchType;

        internal ExceptionRegion(System.Reflection.Metadata.ExceptionRegion exceptionRegion, MetadataState metadataState) : base(new CodeElementKey<ExceptionRegion>(exceptionRegion), metadataState)
        {
            RawMetadata = exceptionRegion;
            _catchType = new Lazy<TypeBase>(() =>
            {
                TypeBase catchType;
                if (RawMetadata.CatchType.Kind == HandleKind.TypeSpecification)
                {
                    catchType = MetadataState.GetCodeElement<TypeSpecification>(RawMetadata.CatchType, this);
                }
                else
                {
                    catchType = (TypeBase)MetadataState.GetCodeElement(RawMetadata.CatchType);
                }
                return catchType;
            });
            FilterOffset = RawMetadata.FilterOffset;
            HandlerLength = RawMetadata.HandlerLength;
            HandlerOffset = RawMetadata.HandlerOffset;
            Kind = RawMetadata.Kind;
            TryLength = RawMetadata.TryLength;
            TryOffset = RawMetadata.TryOffset;
        }

        /// <inheritdoc cref="System.Reflection.Metadata.ExceptionRegion.CatchType" />
        /// <summary>Returns a <see cref="TypeReference" />, <see cref="TypeDefinition" />, or
        ///     <see cref="TypeSpecification" /> if the region represents a catch, null otherwise.</summary>
        public TypeBase CatchType => _catchType.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ExceptionRegion.FilterOffset" />
        public int FilterOffset { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ExceptionRegion.HandlerLength" />
        public int HandlerLength { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ExceptionRegion.HandlerOffset" />
        public int HandlerOffset { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ExceptionRegion.Kind" />
        public ExceptionRegionKind Kind { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ExceptionRegion.TryLength" />
        public int TryLength { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ExceptionRegion.TryOffset" />
        public int TryOffset { get; }

        public System.Reflection.Metadata.ExceptionRegion RawMetadata { get; }
    }
}
