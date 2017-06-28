using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class TypeSpecification : EmptyTypeBase<TypeSpecification, TypeSpecificationHandle, System.Reflection.Metadata.TypeSpecification>
    {
        private readonly GenericContext _genericContext;
        private readonly IManagedCodeElement _referencingCodeElement;
        private Lazy<IEnumerable<CustomAttributeData>> _customAttributes;
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
            _genericContext = genericContext;
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal TypeSpecification(TypeSpecificationHandle handle, IManagedCodeElement referencingCodeElement, MetadataState metadataState) : base(new CodeElementKey<TypeSpecification>(handle, referencingCodeElement), metadataState)
        {
            _referencingCodeElement = referencingCodeElement;
            Initialize();
        }

        public override System.Reflection.Assembly Assembly => MetadataState.AssemblyDefinition;

        public override string FullName => Signature.FullName;

        public override MemberTypes MemberType { get; } = MemberTypes.Custom;

        public override System.Reflection.Module Module => MetadataState.ModuleDefinition;

        public override string Namespace => Signature.Namespace;

        public FieldInfo ReferencingField => _referencingCodeElement as FieldInfo;

        public MethodBase ReferencingMethod => _referencingCodeElement as MethodBase;

        public TypeDefinition ReferencingTypeDefinition => _referencingCodeElement as TypeDefinition;

        public TypeBase Signature => _signature.Value;

        internal override string UndecoratedName => Signature.Name;

        // ReSharper disable once RedundantTypeArgumentsOfMethod
        public override IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<System.Reflection.CustomAttributeData>();

        protected override TypeAttributes GetAttributeFlagsImpl() => Signature.Attributes;

        private void Initialize()
        {
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute, CustomAttributeData>(RawMetadata.GetCustomAttributes());
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
                    genericContext = new GenericContext(this, declaringTypeDefinition?.GenericTypeParameters, ReferencingMethod.GetGenericArguments());
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
