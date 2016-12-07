using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
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

        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public CodeElement Interface => _interface.Value;
    }
}
