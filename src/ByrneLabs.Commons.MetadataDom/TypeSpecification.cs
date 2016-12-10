using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.TypeSpecification" />
    [PublicAPI]
    public class TypeSpecification : TypeBase, ICodeElementWithHandle<TypeSpecificationHandle, System.Reflection.Metadata.TypeSpecification>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<Blob> _signature;

        internal TypeSpecification(TypeSpecificationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetTypeSpecification(metadataHandle);
            _customAttributes = GetLazyCodeElementsWithHandle<CustomAttribute>(MetadataToken.GetCustomAttributes());
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.Signature)));
            //var provider = new DisassemblingTypeProvider();
            //var signature = MetadataToken.DecodeSignature(provider, new DisassemblingGenericContext(new List<string>(), new List<string>()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeSpecification.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public override string Name { get; }
        public override string FullName { get; }

        public override string Namespace { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeSpecification.Signature" />
        public Blob Signature => _signature.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public TypeSpecificationHandle MetadataHandle { get; }

        public System.Reflection.Metadata.TypeSpecification MetadataToken { get; }
    }
}
