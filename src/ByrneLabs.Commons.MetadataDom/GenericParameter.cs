using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter" />
    [PublicAPI]
    public class GenericParameter : TypeBase<GenericParameter, GenericParameterHandle, System.Reflection.Metadata.GenericParameter>
    {
        private Lazy<IEnumerable<GenericParameterConstraint>> _constraints;
        private Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private Lazy<CodeElement> _parent;

        internal GenericParameter(GenericParameter baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState)
        {
            Initialize();
        }

        internal GenericParameter(GenericParameter genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
            Initialize();
        }

        internal GenericParameter(GenericParameterHandle handle, MetadataState metadataState) : base(handle, metadataState)
        {
            Initialize();
        }

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.Attributes" />
        public GenericParameterAttributes Attributes { get; protected set; }

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.GetConstraints" />
        public IEnumerable<GenericParameterConstraint> Constraints => _constraints.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.Index" />
        public int Index { get; private set; }

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.Parent" />
        /// <summary>
        ///     <see cref="TypeDefinition" /> or <see cref="MethodDefinition" />.</summary>
        public CodeElement Parent => _parent.Value;

        private void Initialize()
        {
            Name = AsString(MetadataToken.Name);
            Attributes = MetadataToken.Attributes;
            Index = MetadataToken.Index;
            _parent = MetadataState.GetLazyCodeElement(MetadataToken.Parent);
            _constraints = MetadataState.GetLazyCodeElements<GenericParameterConstraint>(MetadataToken.GetConstraints());
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
            IsGenericParameter = true;
        }
    }
}
