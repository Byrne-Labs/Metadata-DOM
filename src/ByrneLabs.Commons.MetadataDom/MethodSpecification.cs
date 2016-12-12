using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodSpecification" />
    [PublicAPI]
    public class MethodSpecification : RuntimeCodeElement, ICodeElementWithHandle<MethodSpecificationHandle, System.Reflection.Metadata.MethodSpecification>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _method;
        private readonly Lazy<Blob> _signature;

        internal MethodSpecification(MethodSpecificationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetMethodSpecification(metadataHandle);
            _method = MetadataState.GetLazyCodeElement(MetadataToken.Method);
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.Signature)));
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
            //var signature = MetadataToken.DecodeSignature(provider, null);
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodSpecification.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodSpecification.Method" />
        /// <summary><see cref="MethodDefinition" /> or <see cref="MemberReference" /> specifying to which generic method this <see cref="MethodSpecification" /> refers, that is which generic method is it an instantiation of.</summary>
        public CodeElement Method => _method.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodSpecification.Signature" />
        /// <summary></summary>
        public Blob Signature => _signature.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public MethodSpecificationHandle MetadataHandle { get; }

        public System.Reflection.Metadata.MethodSpecification MetadataToken { get; }
    }
}
