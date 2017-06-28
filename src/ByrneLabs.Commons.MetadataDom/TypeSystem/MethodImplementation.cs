using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class MethodImplementation : IManagedCodeElement
    {
        private readonly Lazy<IEnumerable<CustomAttributeData>> _customAttributes;
        private readonly Lazy<MethodBody> _methodBody;
        private readonly Lazy<MethodBase> _methodDeclaration;
        private readonly Lazy<MethodBase> _methodDefinition;
        private readonly Lazy<TypeDefinition> _type;

        internal MethodImplementation(MethodImplementationHandle metadataHandle, MetadataState metadataState)
        {
            Key = new CodeElementKey<MethodImplementation>(metadataHandle);
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.AssemblyReader.GetMethodImplementation(metadataHandle);
            _type = MetadataState.GetLazyCodeElement<TypeDefinition>(RawMetadata.Type);
            _methodDefinition = new Lazy<MethodBase>(() => RawMetadata.MethodBody.Kind == HandleKind.MethodDefinition ? MetadataState.GetCodeElement<MethodBase>(RawMetadata.MethodBody) : throw new ArgumentException($"Unexpected method body type {RawMetadata.MethodBody.Kind}"));
            _methodBody = new Lazy<MethodBody>(() =>
            {
                MethodBody methodBody;
                if (RawMetadata.MethodBody.Kind == HandleKind.MethodDefinition)
                {
                    methodBody = null;
                }
                else
                {
                    methodBody = MetadataState.GetCodeElement<MethodBody>(RawMetadata.MethodBody, new GenericContext(this, Type.GenericTypeParameters, MethodDeclaration.GetGenericArguments()));
                }

                return methodBody;
            });
            _methodDeclaration = new Lazy<MethodBase>(() => RawMetadata.MethodBody.Kind == HandleKind.MemberReference ? MetadataState.GetCodeElement<MethodBase>(RawMetadata.MethodDeclaration, MethodDefinition) : MetadataState.GetCodeElement<MethodBase>(RawMetadata.MethodDeclaration));
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttributeData>(RawMetadata.GetCustomAttributes());
        }

        public IEnumerable<CustomAttributeData> CustomAttributes => _customAttributes.Value;

        public string FullName => $"{MethodDefinition.DeclaringType.FullName}.{Name}";

        public bool IsGenericMethod => MethodDefinition.IsGenericMethod;

        public MethodImplementationHandle MetadataHandle { get; }

        public MethodBody MethodBody => _methodBody.Value;

        public MethodBase MethodDeclaration => _methodDeclaration.Value;

        public MethodBase MethodDefinition => _methodDefinition.Value;

        public string Name => MethodDefinition.Name;

        public System.Reflection.Metadata.MethodImplementation RawMetadata { get; }

        public TypeDefinition Type => _type.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
