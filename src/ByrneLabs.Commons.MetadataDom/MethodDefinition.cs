using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class MethodDefinition : CodeElementWithHandle
    {
        private readonly Lazy<IReadOnlyList<CustomAttribute>> _customAttributes;
        private readonly Lazy<MethodDebugInformation> _debugInformation;
        private readonly Lazy<IReadOnlyList<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<TypeDefinition> _declaringType;
        private readonly Lazy<IReadOnlyList<GenericParameter>> _genericParameters;
        private readonly Lazy<MethodImport> _import;
        private readonly Lazy<string> _name;
        private readonly Lazy<IReadOnlyList<Parameter>> _parameters;
        private readonly Lazy<Blob> _signature;

        internal MethodDefinition(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var methodDefinition = Reader.GetMethodDefinition(metadataHandle);
            _name = new Lazy<string>(() => AsString(methodDefinition.Name));
            Attributes = methodDefinition.Attributes;
            ImplAttributes = methodDefinition.ImplAttributes;
            RelativeVirtualAddress = methodDefinition.RelativeVirtualAddress;
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(methodDefinition.Signature)));
            _customAttributes = new Lazy<IReadOnlyList<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(methodDefinition.GetCustomAttributes()));
            _declaringType = new Lazy<TypeDefinition>(() => GetCodeElement<TypeDefinition>(methodDefinition.GetDeclaringType()));
            _declarativeSecurityAttributes = new Lazy<IReadOnlyList<DeclarativeSecurityAttribute>>(() => GetCodeElements<DeclarativeSecurityAttribute>(methodDefinition.GetDeclarativeSecurityAttributes()));
            _genericParameters = new Lazy<IReadOnlyList<GenericParameter>>(() => GetCodeElements<GenericParameter>(methodDefinition.GetGenericParameters()));
            _import = new Lazy<MethodImport>(() => GetCodeElement<MethodImport>(methodDefinition.GetImport()));
            _parameters = new Lazy<IReadOnlyList<Parameter>>(() => GetCodeElements<Parameter>(methodDefinition.GetParameters()));
            _debugInformation = new Lazy<MethodDebugInformation>(() => !MetadataState.HasDebugMetadata ? null : GetCodeElement<MethodDebugInformation>(metadataHandle.ToDebugInformationHandle()));
        }

        public MethodAttributes Attributes { get; }

        public IReadOnlyList<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public MethodDebugInformation DebugInformation => _debugInformation.Value;

        public IReadOnlyList<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public TypeDefinition DeclaringType => _declaringType.Value;

        public IReadOnlyList<GenericParameter> GenericParameters => _genericParameters.Value;

        public MethodImplAttributes ImplAttributes { get; }

        public MethodImport Import => _import.Value;

        public string Name => _name.Value;

        public IReadOnlyList<Parameter> Parameters => _parameters.Value;

        public int RelativeVirtualAddress { get; }

        public Blob Signature => _signature.Value;
    }
}
