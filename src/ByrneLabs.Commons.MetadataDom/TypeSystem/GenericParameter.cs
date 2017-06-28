using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public class GenericParameter : EmptyTypeBase<GenericParameter, GenericParameterHandle, System.Reflection.Metadata.GenericParameter>
    {
        private Lazy<IEnumerable<GenericParameterConstraint>> _constraints;
        private Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private MethodBase _declaringMethod;
        private TypeBase _declaringType;
        private Lazy<IManagedCodeElement> _parent;

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal GenericParameter(GenericParameter baseType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(baseType, typeElementModifier, metadataState)
        {
            _declaringType = (TypeBase) baseType.DeclaringType;
            _declaringMethod = baseType.DeclaringMethod;
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal GenericParameter(GenericParameter genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
            Initialize();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal GenericParameter(GenericParameterHandle handle, MetadataState metadataState) : base(new CodeElementKey<GenericParameter>(handle), metadataState)
        {
            Initialize();
        }

        public override System.Reflection.Assembly Assembly => MetadataState.AssemblyDefinition;

        public IEnumerable<GenericParameterConstraint> Constraints => _constraints.Value;

        public override MethodBase DeclaringMethod => _declaringMethod;

        public override Type DeclaringType => _declaringType;

        public override GenericParameterAttributes GenericParameterAttributes => RawMetadata.Attributes;

        public int Index => RawMetadata.Index;

        public override bool IsGenericParameter => true;

        public override string Namespace => DeclaringType?.Namespace;

        internal IManagedCodeElement Parent => _parent.Value;

        internal override string UndecoratedName => MetadataState.AssemblyReader.GetString(RawMetadata.Name);

        public override IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<System.Reflection.CustomAttributeData>();

        internal void SetDeclaringMethod(MethodBase declaringMethod)
        {
            _declaringMethod = declaringMethod;
        }

        internal void SetDeclaringType(TypeBase declaringType) => _declaringType = declaringType;

        private void Initialize()
        {
            _parent = MetadataState.GetLazyCodeElement(RawMetadata.Parent);
            _constraints = MetadataState.GetLazyCodeElements<GenericParameterConstraint>(RawMetadata.GetConstraints());
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }
    }
}
