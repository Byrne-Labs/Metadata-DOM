using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

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
            _type = MetadataState.GetLazyCodeElement<TypeDefinition>(RawMetadata.Type);
            _methodDefinition = new Lazy<MethodDefinitionBase>(() => RawMetadata.MethodBody.Kind == HandleKind.MethodDefinition ? MetadataState.GetCodeElement<MethodDefinitionBase>(RawMetadata.MethodBody) : null);
            _methodBody = new Lazy<MethodBody>(() =>
            {
                MethodBody methodBody;
                if (RawMetadata.MethodBody.Kind == HandleKind.MethodDefinition)
                {
                    methodBody = null;
                }
                else
                {
                    methodBody = MetadataState.GetCodeElement<MethodBody>(RawMetadata.MethodBody);
                    methodBody.GenericContext = new GenericContext(Type.GenericTypeParameters, MethodDeclaration.GenericTypeParameters);
                }

                return methodBody;
            });
            _methodDeclaration = MetadataState.GetLazyCodeElement<IMethod>(RawMetadata.MethodDeclaration);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodImplementation.GetCustomAttributes" />
        public override IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public override TypeBase DeclaringType => MethodDefinition.DeclaringType;

        public override string FullName => $"{DeclaringType.FullName}.{Name}";

        public override IEnumerable<GenericParameter> GenericTypeParameters => MethodDefinition.GenericTypeParameters;

        public override bool IsGenericMethod => MethodDefinition.IsGenericMethod;

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
