using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.InterfaceImplementation" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {ImplementingType.FullName,nq} : {Interface.FullName}")]
    public class InterfaceImplementation : RuntimeCodeElement, ICodeElementWithTypedHandle<InterfaceImplementationHandle, System.Reflection.Metadata.InterfaceImplementation>
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;

        internal InterfaceImplementation(InterfaceImplementationHandle metadataHandle, TypeDefinition implementingType, MetadataState metadataState) : base(new CodeElementKey<InterfaceImplementation>(metadataHandle, implementingType), metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetInterfaceImplementation(metadataHandle);
            ImplementingType = implementingType;
            if (RawMetadata.Interface.Kind == HandleKind.TypeSpecification)
            {
                Interface = MetadataState.GetCodeElement<TypeSpecification>(RawMetadata.Interface, ImplementingType);
            }
            else
            {
                Interface = (TypeBase)MetadataState.GetCodeElement(RawMetadata.Interface);
            }
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.InterfaceImplementation.GetCustomAttributes" />
        public ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public TypeDefinition ImplementingType { get; internal set; }

        /// <inheritdoc cref="System.Reflection.Metadata.InterfaceImplementation.Interface" />
        /// <summary>The interface that is implemented <see cref="ImplementingType" />, <see cref="TypeReference" />, or <see cref="TypeSpecification" />
        /// </summary>
        public TypeBase Interface { get; }

        public System.Reflection.Metadata.InterfaceImplementation RawMetadata { get; }

        public InterfaceImplementationHandle MetadataHandle { get; }
    }
}
