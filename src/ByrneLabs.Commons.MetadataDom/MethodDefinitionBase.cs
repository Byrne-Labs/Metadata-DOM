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
    public abstract class MethodDefinitionBase : RuntimeCodeElement, ICodeElementWithHandle<MethodDefinitionHandle, System.Reflection.Metadata.MethodDefinition>, IContainsSourceCode
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<MethodDebugInformation> _debugInformation;
        private readonly Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<TypeDefinition> _declaringType;
        private readonly Lazy<IEnumerable<GenericParameter>> _genericParameters;
        private readonly Lazy<MethodImport> _import;
        private readonly Lazy<MethodBody> _methodBody;
        private readonly Lazy<IEnumerable<Parameter>> _parameters;
        private readonly Lazy<CodeElement> _returnType;

        internal MethodDefinitionBase(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetMethodDefinition(metadataHandle);
            Name = AsString(MetadataToken.Name);
            Attributes = MetadataToken.Attributes;
            ImplAttributes = MetadataToken.ImplAttributes;
            RelativeVirtualAddress = MetadataToken.RelativeVirtualAddress;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
            _declaringType = MetadataState.GetLazyCodeElement<TypeDefinition>(MetadataToken.GetDeclaringType());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(MetadataToken.GetDeclarativeSecurityAttributes());
            _genericParameters = MetadataState.GetLazyCodeElements<GenericParameter>(MetadataToken.GetGenericParameters());
            _import = MetadataState.GetLazyCodeElement<MethodImport>(MetadataToken.GetImport());
            //_methodBody = new Lazy<MethodBody>(() => MetadataToken.RelativeVirtualAddress == 0 ? null : MetadataState.GetCodeElement<MethodBody>(new CodeElementKey<MethodBody>(MetadataToken.RelativeVirtualAddress)));
            _parameters = MetadataState.GetLazyCodeElements<Parameter>(MetadataToken.GetParameters());
            _debugInformation = new Lazy<MethodDebugInformation>(() => !MetadataState.HasDebugMetadata ? null : MetadataState.GetCodeElement<MethodDebugInformation>(metadataHandle.ToDebugInformationHandle()));
            _returnType = new Lazy<CodeElement>(() => MetadataToken.DecodeSignature(MetadataState.SignatureTypeProvider, new CodeElementGenericContext(DeclaringType.GenericParameters, GenericParameters)).ReturnType);
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.Attributes" />
        public MethodAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinitionHandle.ToDebugInformationHandle" />
        /// <summary>Returns a <see cref="MethodDebugInformation" /> corresponding to this handle.</summary>
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

        public MethodBody MethodBody => null; //_methodBody.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetParameters" />
        public IEnumerable<Parameter> Parameters => _parameters.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.RelativeVirtualAddress" />
        public int RelativeVirtualAddress { get; }

        /// <summary>Returns <see cref="TypeDefinition" />, <see cref="TypeReference" />, <see cref="TypeSpecification" />, <see cref="GenericParameter" />, or null when void</summary>
        public CodeElement ReturnType => _returnType.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.Signature" />
        public Handle DowncastMetadataHandle => MetadataHandle;

        public MethodDefinitionHandle MetadataHandle { get; }

        public System.Reflection.Metadata.MethodDefinition MetadataToken { get; }

        public Document Document { get; }

        public string SourceCode => DebugInformation?.SourceCode;
    }
}
