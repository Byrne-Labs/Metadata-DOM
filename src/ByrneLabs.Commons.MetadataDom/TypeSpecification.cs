﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.TypeSpecification" />
    //[PublicAPI]
    public class TypeSpecification : TypeBase<TypeSpecification, TypeSpecificationHandle, System.Reflection.Metadata.TypeSpecification>
    {
        private Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private Lazy<TypeBase> _signature;

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

        public override IAssembly Assembly => MetadataState.AssemblyDefinition;

        /// <inheritdoc cref="System.Reflection.Metadata.TypeSpecification.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public override TypeBase DeclaringType { get; } = null;

        public override string FullName => Signature.FullName;

        public override bool IsGenericParameter { get; } = false;

        public override MemberTypes MemberType { get; } = MemberTypes.Custom;

        public override string Namespace => Signature.Namespace;

        public MethodDefinition ParentMethodDefinition { get; internal set; }

        public TypeDefinition ParentTypeDefinition { get; internal set; }

        public TypeBase Signature => _signature.Value;

        internal override string UndecoratedName => Signature.Name;

        private void Initialize()
        {
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
            _signature = new Lazy<TypeBase>(() =>
            {
                GenericContext genericContext;
                if (ParentTypeDefinition != null)
                {
                    genericContext = new GenericContext(ParentTypeDefinition.GenericTypeParameters, null);
                }
                else if (ParentMethodDefinition != null)
                {
                    genericContext = new GenericContext(null, ParentMethodDefinition.GenericTypeParameters);
                }
                else
                {
                    throw new InvalidOperationException("No generic type parameters found for type specification");
                }

                var signature = MetadataToken.DecodeSignature(MetadataState.TypeProvider, genericContext);
                return signature;
            });
        }
    }
}
