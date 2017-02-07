using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.TypeSpecification" />
    //[PublicAPI]
    public class TypeSpecification : TypeBase<TypeSpecification, TypeSpecificationHandle, System.Reflection.Metadata.TypeSpecification>
    {
        private readonly GenericContext _genericContext;
        private readonly RuntimeCodeElement _referencingCodeElement;
        private Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private Lazy<TypeBase> _signature;

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal TypeSpecification(TypeSpecification baseType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(baseType, typeElementModifier, metadataState)
        {
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal TypeSpecification(TypeSpecification genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal TypeSpecification(TypeSpecificationHandle handle, GenericContext genericContext, MetadataState metadataState) : base(new CodeElementKey<TypeSpecification>(handle, genericContext), metadataState)
        {
            MetadataHandle = handle;
            _genericContext = genericContext;
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal TypeSpecification(TypeSpecificationHandle handle, RuntimeCodeElement referencingCodeElement, MetadataState metadataState) : base(new CodeElementKey<TypeSpecification>(handle, referencingCodeElement), metadataState)
        {
            MetadataHandle = handle;
            _referencingCodeElement = referencingCodeElement;
            Initialize();
        }

        public override IAssembly Assembly => MetadataState.AssemblyDefinition;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeSpecification.GetCustomAttributes" />
        public override ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public override TypeBase DeclaringType { get; } = null;

        public override string FullName => Signature.FullName;

        public override bool IsGenericParameter { get; } = false;

        public override MemberTypes MemberType { get; } = MemberTypes.Custom;

        public override TypeSpecificationHandle MetadataHandle { get; }

        public override string Namespace => Signature.Namespace;

        public FieldReference ReferencingField => _referencingCodeElement as FieldReference;

        public IMethodBase ReferencingMethod => _referencingCodeElement as IMethodBase;

        public TypeDefinition ReferencingTypeDefinition => _referencingCodeElement as TypeDefinition;

        public TypeBase Signature => _signature.Value;

        protected override string MetadataNamespace { get; } = null;

        internal override string UndecoratedName => Signature.Name;

        private void Initialize()
        {
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            _signature = new Lazy<TypeBase>(() =>
            {
                GenericContext genericContext;
                if (_genericContext != null)
                {
                    genericContext = _genericContext;
                }
                else if (ReferencingTypeDefinition != null)
                {
                    genericContext = new GenericContext(this, ReferencingTypeDefinition.GenericTypeParameters, null);
                }
                else if (ReferencingMethod != null)
                {
                    var declaringTypeDefinition = ReferencingMethod.DeclaringType as TypeDefinition;
                    genericContext = new GenericContext(this, declaringTypeDefinition?.GenericTypeParameters, ReferencingMethod.GenericTypeParameters);
                }
                else if (ReferencingField?.FieldType is TypeDefinition)
                {
                    /*
                     * For reasons I do not understand, the signature decoder sometimes calls TypeProvider.GetGenericMethodParameter and other times calls TypeProvider.GetGenericTypeParameter.  Passing the generic 
                     * paramaters as both type parameters and method parameters keeps an exception from being thrown but may be incorrect. -- Jonathan Byrne 01/30/2017
                     */
                    genericContext = new GenericContext(this, ((TypeDefinition) ReferencingField.FieldType).GenericTypeParameters, ((TypeDefinition) ReferencingField.FieldType).GenericTypeParameters);
                }
                else
                {
                    genericContext = new GenericContext(this);
                }

                var signature = RawMetadata.DecodeSignature(MetadataState.TypeProvider, genericContext);
                return signature;
            });
        }
    }
}
