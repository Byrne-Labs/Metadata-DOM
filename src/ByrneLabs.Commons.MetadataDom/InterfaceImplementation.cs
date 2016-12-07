using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.InterfaceImplementation" />
    public class InterfaceImplementation : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _interface;

        internal InterfaceImplementation(InterfaceImplementationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var interfaceImplementation = Reader.GetInterfaceImplementation(metadataHandle);
            _interface = new Lazy<CodeElement>(() => GetCodeElement(interfaceImplementation.Interface));
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(interfaceImplementation.GetCustomAttributes()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.InterfaceImplementation.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.InterfaceImplementation.Interface" />
        /// <summary>The interface that is implemented
        ///     <see cref="T:ByrneLabs.Commons.MetadataDom.TypeDefinition" />, <see cref="T:ByrneLabs.Commons.MetadataDom.TypeReference" />, or
        ///     <see cref="T:ByrneLabs.Commons.MetadataDom.TypeSpecification" />
        /// </summary>
        public CodeElement Interface => _interface.Value;
    }
}
