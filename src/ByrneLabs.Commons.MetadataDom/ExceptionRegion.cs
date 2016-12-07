using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
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

        public CodeElement CatchType => _catchType.Value;

        public int FilterOffset { get; }

        public int HandlerLength { get; }

        public int HandlerOffset { get; }

        public ExceptionRegionKind Kind { get; }

        public int TryLength { get; }

        public int TryOffset { get; }
    }
}
