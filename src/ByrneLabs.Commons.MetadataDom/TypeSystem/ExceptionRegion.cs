using System;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using ExceptionHandlingClauseToExpose = System.Reflection.ExceptionHandlingClause;
using ExceptionHandlingClauseOptionsToExpose = System.Reflection.ExceptionHandlingClauseOptions;
using TypeToExpose = System.Type;

#else
using ExceptionHandlingClauseToExpose = ByrneLabs.Commons.MetadataDom.ExceptionHandlingClause;
using ExceptionHandlingClauseOptionsToExpose = ByrneLabs.Commons.MetadataDom.ExceptionHandlingClauseOptions;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class ExceptionRegion : ExceptionHandlingClauseToExpose, IManagedCodeElement
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
            Flags = (ExceptionHandlingClauseOptionsToExpose) RawMetadata.Kind;
            TryLength = RawMetadata.TryLength;
            TryOffset = RawMetadata.TryOffset;
        }

        public override TypeToExpose CatchType => _catchType.Value;

        public override int FilterOffset { get; }

        public override ExceptionHandlingClauseOptionsToExpose Flags { get; }

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
