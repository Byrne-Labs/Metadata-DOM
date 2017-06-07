using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using TypeToExpose = System.Type;
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;

#else
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class GenericParameterConstraint : MetadataDom.GenericParameterConstraint, IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<GenericParameter> _parameter;
        private readonly Lazy<TypeBase> _type;

        internal GenericParameterConstraint(GenericParameterConstraintHandle metadataHandle, MetadataState metadataState)
        {
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            Key = new CodeElementKey<GenericParameterConstraint>(metadataHandle);
            RawMetadata = MetadataState.AssemblyReader.GetGenericParameterConstraint(metadataHandle);
            _type = new Lazy<TypeBase>(() =>
            {
                TypeBase constrainedType;
                switch (RawMetadata.Type.Kind)
                {
                    case HandleKind.TypeSpecification:
                        constrainedType = MetadataState.GetCodeElement<TypeSpecification>(RawMetadata.Type, _parameter.Value.Parent);
                        break;
                    case HandleKind.TypeDefinition:
                        constrainedType = MetadataState.GetCodeElement<TypeDefinition>(RawMetadata.Type);
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected constrained type {RawMetadata.Type.Kind}");
                }

                return constrainedType;
            });
            _parameter = MetadataState.GetLazyCodeElement<GenericParameter>(RawMetadata.Parameter);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        public override IEnumerable<CustomAttributeDataToExpose> CustomAttributes => _customAttributes.Value;

        public GenericParameterConstraintHandle MetadataHandle { get; }

        public override TypeToExpose Parameter => _parameter.Value;

        public System.Reflection.Metadata.GenericParameterConstraint RawMetadata { get; }

        public override TypeToExpose Type => _type.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
