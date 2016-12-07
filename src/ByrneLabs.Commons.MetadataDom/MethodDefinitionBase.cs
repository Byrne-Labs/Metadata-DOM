using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    [DebuggerDisplay("{DeclaringType.Namespace}.{DeclaringType.Name}.{Name}")]
    public abstract class MethodDefinitionBase : CodeElementWithHandle, IContainsSourceCode
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<MethodDebugInformation> _debugInformation;
        private readonly Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<TypeDefinition> _declaringType;
        private readonly Lazy<IEnumerable<GenericParameter>> _genericParameters;
        private readonly Lazy<MethodImport> _import;
        private readonly Lazy<MethodBody> _methodBody;
        private readonly Lazy<string> _name;
        private readonly Lazy<IEnumerable<Parameter>> _parameters;
        private readonly Lazy<Blob> _signature;

        internal MethodDefinitionBase(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var methodDefinition = Reader.GetMethodDefinition(metadataHandle);
            _name = new Lazy<string>(() => AsString(methodDefinition.Name));
            Attributes = methodDefinition.Attributes;
            ImplAttributes = methodDefinition.ImplAttributes;
            RelativeVirtualAddress = methodDefinition.RelativeVirtualAddress;
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(methodDefinition.Signature)));
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(methodDefinition.GetCustomAttributes()));
            _declaringType = new Lazy<TypeDefinition>(() => GetCodeElement<TypeDefinition>(methodDefinition.GetDeclaringType()));
            _declarativeSecurityAttributes = new Lazy<IEnumerable<DeclarativeSecurityAttribute>>(() => GetCodeElements<DeclarativeSecurityAttribute>(methodDefinition.GetDeclarativeSecurityAttributes()));
            _genericParameters = new Lazy<IEnumerable<GenericParameter>>(() => GetCodeElements<GenericParameter>(methodDefinition.GetGenericParameters()));
            _import = new Lazy<MethodImport>(() => GetCodeElement<MethodImport>(new HandlelessCodeElementKey<MethodImport>(methodDefinition.GetImport())));
            _methodBody = new Lazy<MethodBody>(() => methodDefinition.RelativeVirtualAddress == 0 ? null : GetCodeElement<MethodBody>(new HandlelessCodeElementKey<MethodBody>(methodDefinition.RelativeVirtualAddress)));
            _parameters = new Lazy<IEnumerable<Parameter>>(() => GetCodeElements<Parameter>(methodDefinition.GetParameters()));
            _debugInformation = new Lazy<MethodDebugInformation>(() => !MetadataState.HasDebugMetadata ? null : GetCodeElement<MethodDebugInformation>(metadataHandle.ToDebugInformationHandle()));
        }

        public MethodAttributes Attributes { get; }

        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public MethodDebugInformation DebugInformation => _debugInformation.Value;

        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public TypeDefinition DeclaringType => _declaringType.Value;

        public IEnumerable<GenericParameter> GenericParameters => _genericParameters.Value;

        public MethodImplAttributes ImplAttributes { get; }

        public MethodImport Import => _import.Value;

        public MethodBody MethodBody => _methodBody.Value;

        public string Name => _name.Value;

        public IEnumerable<Parameter> Parameters => _parameters.Value;

        public int RelativeVirtualAddress { get; }

        public Blob Signature => _signature.Value;

        public string SourceCode => DebugInformation?.SourceCode;

        public string SourceFile => DebugInformation?.SourceFile;
    }
}
