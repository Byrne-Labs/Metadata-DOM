using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.TypeSpecification" />
    public class TypeSpecification : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<Blob> _signature;

        internal TypeSpecification(TypeSpecificationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var typeSpecification = Reader.GetTypeSpecification(metadataHandle);
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(typeSpecification.Signature)));
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(typeSpecification.GetCustomAttributes()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeSpecification.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeSpecification.Signature" />
        public Blob Signature => _signature.Value;
    }
}
