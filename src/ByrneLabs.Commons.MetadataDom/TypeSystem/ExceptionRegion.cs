using System;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class ExceptionRegion : ExceptionHandlingClause, IManagedCodeElement
    {
        private readonly Lazy<TypeBase> _catchType;

        internal ExceptionRegion(System.Reflection.Metadata.ExceptionRegion exceptionRegion, MetadataState metadataState)
        {
            MetadataState = metadataState;
            Key = new CodeElementKey<Constant>(exceptionRegion);
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
                    catchType = (TypeBase) MetadataState.GetCodeElement(RawMetadata.CatchType);
                }
                return catchType;
            });
            FilterOffset = RawMetadata.FilterOffset;
            HandlerLength = RawMetadata.HandlerLength;
            HandlerOffset = RawMetadata.HandlerOffset;
            Flags = (ExceptionHandlingClauseOptions) RawMetadata.Kind;
            TryLength = RawMetadata.TryLength;
            TryOffset = RawMetadata.TryOffset;
        }

        public override Type CatchType => _catchType.Value;

        public override int FilterOffset { get; }

        public override ExceptionHandlingClauseOptions Flags { get; }

        public override int HandlerLength { get; }

        public override int HandlerOffset { get; }

        public System.Reflection.Metadata.ExceptionRegion RawMetadata { get; }

        public override int TryLength { get; }

        public override int TryOffset { get; }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
