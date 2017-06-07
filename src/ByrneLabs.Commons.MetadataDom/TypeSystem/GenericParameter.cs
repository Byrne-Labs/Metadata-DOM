using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using AssemblyToExpose = System.Reflection.Assembly;
using MethodBaseToExpose = System.Reflection.MethodBase;

#else
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using AssemblyToExpose = ByrneLabs.Commons.MetadataDom.Assembly;
using MethodBaseToExpose = ByrneLabs.Commons.MetadataDom.MethodBase;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public class GenericParameter : EmptyTypeBase<GenericParameter, GenericParameterHandle, System.Reflection.Metadata.GenericParameter>
    {
        private Lazy<ImmutableArray<GenericParameterConstraint>> _constraints;
        private Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private MethodBaseToExpose _declaringMethod;
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

        public override AssemblyToExpose Assembly => MetadataState.AssemblyDefinition;

        public ImmutableArray<GenericParameterConstraint> Constraints => _constraints.Value;

        public override MethodBaseToExpose DeclaringMethod => _declaringMethod;

        public override TypeToExpose DeclaringType => _declaringType;

        public override GenericParameterAttributes GenericParameterAttributes => RawMetadata.Attributes;

        public int Index => RawMetadata.Index;

        public override bool IsGenericParameter => true;

        public override string Namespace => DeclaringType?.Namespace;

        internal IManagedCodeElement Parent => _parent.Value;

        internal override string UndecoratedName => MetadataState.AssemblyReader.GetString(RawMetadata.Name);

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<CustomAttributeDataToExpose>();

        internal void SetDeclaringMethod(MethodBaseToExpose declaringMethod)
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
