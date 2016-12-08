using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation" />
    [PublicAPI]
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

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation.MethodBody" />
        public CodeElement MethodBody => _methodBody.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation.MethodDeclaration" />
        public CodeElement MethodDeclaration => _methodDeclaration.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation.Type" />
        public TypeDefinition Type => _type.Value;
    }
}
