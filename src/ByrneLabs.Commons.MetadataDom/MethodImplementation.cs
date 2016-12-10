using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation" />
    [PublicAPI]
    public class MethodImplementation : RuntimeCodeElement, ICodeElementWithHandle<MethodImplementationHandle, System.Reflection.Metadata.MethodImplementation>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<MethodBody> _methodBody;
        private readonly Lazy<CodeElement> _methodDeclaration;
        private readonly Lazy<MethodDefinitionBase> _methodDefinition;
        private readonly Lazy<TypeDefinition> _type;

        internal MethodImplementation(MethodImplementationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetMethodImplementation(metadataHandle);
            _type = GetLazyCodeElementWithHandle<TypeDefinition>(MetadataToken.Type);
            _methodDefinition = new Lazy<MethodDefinitionBase>(() => MetadataToken.MethodBody.Kind == HandleKind.MethodDefinition ? GetCodeElementWithHandle<MethodDefinitionBase>(MetadataToken.MethodBody) : null);
            _methodBody = new Lazy<MethodBody>(() => MetadataToken.MethodBody.Kind == HandleKind.MethodDefinition ? null : GetCodeElementWithHandle<MethodBody>(MetadataToken.MethodBody));
            _methodDeclaration = GetLazyCodeElementWithHandle(MetadataToken.MethodDeclaration);
            _customAttributes = GetLazyCodeElementsWithoutHandle<CustomAttribute>(MetadataToken.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation.MethodBody" />
        public CodeElement MethodBody => _methodBody.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation.MethodDeclaration" />
        public CodeElement MethodDeclaration => _methodDeclaration.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation.Type" />
        public TypeDefinition Type => _type.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public MethodImplementationHandle MetadataHandle { get; }

        public System.Reflection.Metadata.MethodImplementation MetadataToken { get; }
    }
}
