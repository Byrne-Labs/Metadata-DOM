using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class TypeSpecification : CodeElementWithHandle
    {
        private readonly Lazy<IReadOnlyList<CustomAttribute>> _customAttributes;
        private readonly Lazy<Blob> _signature;

        internal TypeSpecification(TypeSpecificationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var typeSpecification = Reader.GetTypeSpecification(metadataHandle);
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(typeSpecification.Signature)));
            _customAttributes = new Lazy<IReadOnlyList<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(typeSpecification.GetCustomAttributes()));
        }

        public IReadOnlyList<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public Blob Signature => _signature.Value;
    }
}
