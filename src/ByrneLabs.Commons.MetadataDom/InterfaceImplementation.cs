using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class InterfaceImplementation : CodeElementWithHandle
    {
        private readonly Lazy<IReadOnlyList<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _interface;

        internal InterfaceImplementation(InterfaceImplementationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var interfaceImplementation = Reader.GetInterfaceImplementation(metadataHandle);
            _interface = new Lazy<CodeElement>(() => GetCodeElement(interfaceImplementation.Interface));
            _customAttributes = new Lazy<IReadOnlyList<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(interfaceImplementation.GetCustomAttributes()));
        }

        public IReadOnlyList<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public CodeElement Interface => _interface.Value;
    }
}
