using System;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ExceptionRegion" />
    [PublicAPI]
    public class ExceptionRegion : RuntimeCodeElement, ICodeElementWithToken<System.Reflection.Metadata.ExceptionRegion>
    {
        private readonly Lazy<TypeBase> _catchType;

        internal ExceptionRegion(System.Reflection.Metadata.ExceptionRegion exceptionRegion, MetadataState metadataState) : base(new HandlelessCodeElementKey<ExceptionRegion>(exceptionRegion), metadataState)
        {
            MetadataToken = exceptionRegion;
            _catchType = GetLazyCodeElementWithHandle<TypeBase>(MetadataToken.CatchType);
            FilterOffset = MetadataToken.FilterOffset;
            HandlerLength = MetadataToken.HandlerLength;
            HandlerOffset = MetadataToken.HandlerOffset;
            Kind = MetadataToken.Kind;
            TryLength = MetadataToken.TryLength;
            TryOffset = MetadataToken.TryOffset;
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

        public System.Reflection.Metadata.ExceptionRegion MetadataToken { get; }
    }
}
