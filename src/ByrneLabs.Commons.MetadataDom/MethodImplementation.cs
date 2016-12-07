using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class MethodImplementation : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _methodBody;
        private readonly Lazy<CodeElement> _methodDeclaration;
        private readonly Lazy<TypeDefinition> _type;

        internal MethodImplementation(MethodImplementationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var methodImplementation = Reader.GetMethodImplementation(metadataHandle);
            _type = new Lazy<TypeDefinition>(() => GetCodeElement<TypeDefinition>(methodImplementation.Type));
            _methodBody = new Lazy<CodeElement>(() => GetCodeElement(methodImplementation.MethodBody));
            _methodDeclaration = new Lazy<CodeElement>(() => GetCodeElement(methodImplementation.MethodDeclaration));
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(methodImplementation.GetCustomAttributes()));
        }

        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public CodeElement MethodBody => _methodBody.Value;

        public CodeElement MethodDeclaration => _methodDeclaration.Value;

        public TypeDefinition Type => _type.Value;
    }
}
