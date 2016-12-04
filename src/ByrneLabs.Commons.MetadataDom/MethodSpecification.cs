using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class MethodSpecification : CodeElementWithHandle
    {
        private readonly Lazy<IReadOnlyList<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _method;
        private readonly Lazy<Blob> _signature;

        internal MethodSpecification(MethodSpecificationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var methodSpecification = Reader.GetMethodSpecification(metadataHandle);
            _method = new Lazy<CodeElement>(() => GetCodeElement(methodSpecification.Method));
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(methodSpecification.Signature)));
            _customAttributes = new Lazy<IReadOnlyList<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(methodSpecification.GetCustomAttributes()));
        }

        public IReadOnlyList<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public CodeElement Method => _method.Value;

        public Blob Signature => _signature.Value;
    }
}
