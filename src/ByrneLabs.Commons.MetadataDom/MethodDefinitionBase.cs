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
        private readonly Lazy<Blob> _signature;

        internal MethodDefinitionBase(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetMethodDefinition(metadataHandle);
            Name = AsString(MetadataToken.Name);
            Attributes = MetadataToken.Attributes;
            ImplAttributes = MetadataToken.ImplAttributes;
            RelativeVirtualAddress = MetadataToken.RelativeVirtualAddress;
            _signature = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.Signature)));
            _customAttributes = GetLazyCodeElementsWithHandle<CustomAttribute>(MetadataToken.GetCustomAttributes());
            _declaringType = GetLazyCodeElementWithHandle<TypeDefinition>(MetadataToken.GetDeclaringType());
            _declarativeSecurityAttributes = GetLazyCodeElementsWithHandle<DeclarativeSecurityAttribute>(MetadataToken.GetDeclarativeSecurityAttributes());
            _genericParameters = GetLazyCodeElementsWithHandle<GenericParameter>(MetadataToken.GetGenericParameters());
            _import = GetLazyCodeElementWithoutHandle<MethodImport>(MetadataToken.GetImport());
            _methodBody = new Lazy<MethodBody>(() => MetadataToken.RelativeVirtualAddress == 0 ? null : GetCodeElementWithHandle<MethodBody>(new HandlelessCodeElementKey<MethodBody>(MetadataToken.RelativeVirtualAddress)));
            _parameters = GetLazyCodeElementsWithHandle<Parameter>(MetadataToken.GetParameters());
            _debugInformation = new Lazy<MethodDebugInformation>(() => !MetadataState.HasDebugMetadata ? null : GetCodeElementWithHandle<MethodDebugInformation>(metadataHandle.ToDebugInformationHandle()));
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

        public MethodBody MethodBody => _methodBody.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetParameters" />
        public IEnumerable<Parameter> Parameters => _parameters.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.RelativeVirtualAddress" />
        public int RelativeVirtualAddress { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.Signature" />
        public Blob Signature => _signature.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public MethodDefinitionHandle MetadataHandle { get; }

        public System.Reflection.Metadata.MethodDefinition MetadataToken { get; }

        public Document Document { get; }

        public string SourceCode => DebugInformation?.SourceCode;

    }
}
