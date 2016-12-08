using System;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ExceptionRegion" />
    [PublicAPI]
    public class ExceptionRegion : CodeElementWithoutHandle
    {
        private readonly Lazy<CodeElement> _catchType;

        internal ExceptionRegion(System.Reflection.Metadata.ExceptionRegion exceptionRegion, MetadataState metadataState) : base(new HandlelessCodeElementKey<ExceptionRegion>(exceptionRegion), metadataState)
        {
            _catchType = GetLazyCodeElement(exceptionRegion.CatchType);
            FilterOffset = exceptionRegion.FilterOffset;
            HandlerLength = exceptionRegion.HandlerLength;
            HandlerOffset = exceptionRegion.HandlerOffset;
            Kind = exceptionRegion.Kind;
            TryLength = exceptionRegion.TryLength;
            TryOffset = exceptionRegion.TryOffset;
        }

        /// <inheritdoc cref="System.Reflection.Metadata.ExceptionRegion.CatchType" />
        /// <summary>Returns a <see cref="T:ByrneLabs.Commons.MetadataDom.TypeReference" />, <see cref="T:ByrneLabs.Commons.MetadataDom.TypeDefinition" />, or
        ///     <see cref="T:ByrneLabs.Commons.MetadataDom.TypeSpecification" /> if the region represents a catch, null otherwise.</summary>
        public CodeElement CatchType => _catchType.Value;

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
    }
}
