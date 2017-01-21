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
        private readonly Lazy<TypeBase> _interface;

        internal InterfaceImplementation(InterfaceImplementationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetInterfaceImplementation(metadataHandle);
            _interface = new Lazy<TypeBase>(() =>
            {
                var interfaceDefinition = (TypeBase) MetadataState.GetCodeElement(RawMetadata.Interface);
                var typeSpecification = interfaceDefinition as TypeSpecification;
                if (typeSpecification != null)
                {
                    typeSpecification.ParentTypeDefinition = ImplementingType;
                }
                return interfaceDefinition;
            });
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.InterfaceImplementation.GetCustomAttributes" />
        public ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public TypeDefinition ImplementingType { get; internal set; }

        /// <inheritdoc cref="System.Reflection.Metadata.InterfaceImplementation.Interface" />
        /// <summary>The interface that is implemented <see cref="ImplementingType" />, <see cref="TypeReference" />, or <see cref="TypeSpecification" />
        /// </summary>
        public TypeBase Interface => _interface.Value;

        public System.Reflection.Metadata.InterfaceImplementation RawMetadata { get; }

        public InterfaceImplementationHandle MetadataHandle { get; }
    }
}
