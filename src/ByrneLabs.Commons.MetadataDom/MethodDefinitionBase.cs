using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition" />
    [DebuggerDisplay("{DeclaringType.Namespace}.{DeclaringType.Name}.{Name}")]
    [PublicAPI]
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

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.Attributes" />
        public MethodAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinitionHandle.ToDebugInformationHandle" />
        /// <summary>Returns a <see cref="ByrneLabs.Commons.MetadataDom.MethodDebugInformation" /> corresponding to this handle.</summary>
        /// <remarks></remarks>
        public MethodDebugInformation DebugInformation => _debugInformation.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetDeclarativeSecurityAttributes" />
        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetDeclaringType" />
        public TypeDefinition DeclaringType => _declaringType.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetGenericParameters" />
        public IEnumerable<GenericParameter> GenericParameters => _genericParameters.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.ImplAttributes" />
        public MethodImplAttributes ImplAttributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetImport" />
        public MethodImport Import => _import.Value;

        public MethodBody MethodBody => _methodBody.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.Name" />
        public string Name => _name.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetParameters" />
        public IEnumerable<Parameter> Parameters => _parameters.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.RelativeVirtualAddress" />
        public int RelativeVirtualAddress { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.Signature" />
        public Blob Signature => _signature.Value;

        public string SourceCode => DebugInformation?.SourceCode;

        public string SourceFile => DebugInformation?.SourceFile;
    }
}
