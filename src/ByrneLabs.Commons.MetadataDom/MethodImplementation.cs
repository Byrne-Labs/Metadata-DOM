using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation" />
    //[PublicAPI]
    public class MethodImplementation : MethodBase<MethodImplementation, MethodImplementationHandle, System.Reflection.Metadata.MethodImplementation>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<MethodBody> _methodBody;
        private readonly Lazy<IMethod> _methodDeclaration;
        private readonly Lazy<MethodDefinitionBase> _methodDefinition;
        private readonly Lazy<TypeDefinition> _type;

        internal MethodImplementation(MethodImplementationHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            _type = MetadataState.GetLazyCodeElement<TypeDefinition>(MetadataToken.Type);
            _methodDefinition = new Lazy<MethodDefinitionBase>(() => MetadataToken.MethodBody.Kind == HandleKind.MethodDefinition ? MetadataState.GetCodeElement<MethodDefinitionBase>(MetadataToken.MethodBody) : null);
            _methodBody = new Lazy<MethodBody>(() =>
            {
                MethodBody methodBody;
                if (MetadataToken.MethodBody.Kind == HandleKind.MethodDefinition)
                {
                    methodBody = null;
                }
                else
                {
                    methodBody = MetadataState.GetCodeElement<MethodBody>(MetadataToken.MethodBody);
                    methodBody.GenericContext = new GenericContext(Type.GenericTypeArguments, _methodDeclaration.Value.GenericArguments);
                }

                return methodBody;
            });
            _methodDeclaration = MetadataState.GetLazyCodeElement<IMethod>(MetadataToken.MethodDeclaration);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation.GetCustomAttributes" />
        public override IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public override TypeBase DeclaringType => MethodDefinition.DeclaringType;

        public override string FullName => $"{DeclaringType.FullName}.{Name}";

        public override IEnumerable<TypeBase> GenericArguments => MethodDefinition.GenericArguments;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation.MethodBody" />
        public CodeElement MethodBody => _methodBody.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation.MethodDeclaration" />
        public IMethod MethodDeclaration => _methodDeclaration.Value;

        public MethodDefinitionBase MethodDefinition => _methodDefinition.Value;

        public override string Name => MethodDefinition.Name;

        public override IEnumerable<IParameter> Parameters => MethodDefinition.Parameters;

        public override string TextSignature => MethodDefinition.TextSignature;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation.Type" />
        public TypeDefinition Type => _type.Value;
    }
}
