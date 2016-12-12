using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.TypeSpecification" />
    [PublicAPI]
    public class TypeSpecification : TypeBase<TypeSpecification, TypeSpecificationHandle, System.Reflection.Metadata.TypeSpecification>
    {
        private Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private Lazy<Blob> _signature;

        internal TypeSpecification(TypeSpecification baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState)
        {
            Initialize();
        }

        internal TypeSpecification(TypeSpecification genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
            Initialize();
        }

        internal TypeSpecification(TypeSpecificationHandle handle, MetadataState metadataState) : base(handle, metadataState)
        {
            Initialize();
        }

        /// <inheritdoc cref="System.Reflection.Metadata.TypeSpecification.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeSpecification.Signature" />
        public Blob Signature => _signature.Value;

        private void Initialize()
        {
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.Signature)));
           // var signature = MetadataToken.DecodeSignature(MetadataState.SignatureTypeProvider, new CodeElementGenericContext(new List<GenericParameter>(), new List<GenericParameter>()));
        }
    }
}
