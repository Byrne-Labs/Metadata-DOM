using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.InterfaceImplementation" />
    [PublicAPI]
    public class InterfaceImplementation : RuntimeCodeElement, ICodeElementWithHandle<InterfaceImplementationHandle, System.Reflection.Metadata.InterfaceImplementation>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<TypeBase> _interface;

        internal InterfaceImplementation(InterfaceImplementationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetInterfaceImplementation(metadataHandle);
            _interface = new Lazy<TypeBase>(() => (TypeBase)MetadataState.GetCodeElement(MetadataToken.Interface));
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.InterfaceImplementation.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.InterfaceImplementation.Interface" />
        /// <summary>The interface that is implemented <see cref="TypeDefinition" />, <see cref="TypeReference" />, or <see cref="TypeSpecification" />
        /// </summary>
        public TypeBase Interface => _interface.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public InterfaceImplementationHandle MetadataHandle { get; }

        public System.Reflection.Metadata.InterfaceImplementation MetadataToken { get; }
    }
}
