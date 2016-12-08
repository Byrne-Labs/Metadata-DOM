using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodSpecification" />
    [PublicAPI]
    public class MethodSpecification : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _method;
        private readonly Lazy<Blob> _signature;

        internal MethodSpecification(MethodSpecificationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var methodSpecification = Reader.GetMethodSpecification(metadataHandle);
            _method = new Lazy<CodeElement>(() => GetCodeElement(methodSpecification.Method));
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(methodSpecification.Signature)));
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(methodSpecification.GetCustomAttributes()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodSpecification.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodSpecification.Method" />
        /// <summary>
        ///     <see cref="ByrneLabs.Commons.MetadataDom.MethodDefinition" /> or <see cref="ByrneLabs.Commons.MetadataDom.MemberReference" /> specifying to which generic method this
        ///     <see cref="ByrneLabs.Commons.MetadataDom.MethodSpecification" /> refers, that is which generic method is it an instantiation of.</summary>
        public CodeElement Method => _method.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodSpecification.Signature" />
        /// <summary></summary>
        public Blob Signature => _signature.Value;
    }
}
