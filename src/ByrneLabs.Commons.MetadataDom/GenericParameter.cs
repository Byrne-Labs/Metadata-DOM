using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public class GenericParameter : TypeBase<GenericParameter, GenericParameterHandle, System.Reflection.Metadata.GenericParameter>
    {
        private Lazy<IEnumerable<GenericParameterConstraint>> _constraints;
        private Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private TypeBase _declaringType;
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

        public override IAssembly Assembly => MetadataState.AssemblyDefinition;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.Attributes" />
        public GenericParameterAttributes Attributes { get; protected set; }

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.GetConstraints" />
        public IEnumerable<GenericParameterConstraint> Constraints => _constraints.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public MethodDefinition DeclaringMethod { get; internal set; }

        public override TypeBase DeclaringType => _declaringType;

        public override string FullName => null;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.Index" />
        public int Index { get; internal set; }

        public override bool IsGenericParameter { get; } = true;

        public override MemberTypes MemberType { get; } = MemberTypes.Custom;

        public override string Namespace => DeclaringType?.Namespace;

        /// <inheritdoc cref="System.Reflection.Metadata.GenericParameter.Parent" />
        /// <summary>
        ///     <see cref="TypeDefinition" /> or <see cref="MethodDefinition" />.</summary>
        public CodeElement Parent => _parent.Value;

        internal override string UndecoratedName => AsString(MetadataToken.Name);

        internal void SetDeclaringType(TypeBase declaringType) => _declaringType = declaringType;

        private void Initialize()
        {
            Attributes = MetadataToken.Attributes;
            Index = MetadataToken.Index;
            _parent = MetadataState.GetLazyCodeElement(MetadataToken.Parent);
            _constraints = MetadataState.GetLazyCodeElements<GenericParameterConstraint>(MetadataToken.GetConstraints());
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
        }
    }
}
