using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition" />
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {TextSignature}")]
    //[PublicAPI]
    public abstract class MethodDefinitionBase : MethodBase<MethodDefinitionBase, MethodDefinitionHandle, System.Reflection.Metadata.MethodDefinition>, IContainsSourceCode
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<MethodDebugInformation> _debugInformation;
        private readonly Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<TypeDefinition> _declaringType;
        private readonly Lazy<IEnumerable<GenericParameter>> _genericParameters;
        private readonly Lazy<MethodImport> _import;
        private readonly Lazy<MethodBody> _methodBody;
        private readonly Lazy<IEnumerable<Parameter>> _parameters;
        private readonly Lazy<MethodSignature<TypeBase>> _signature;

        internal MethodDefinitionBase(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            Name = AsString(MetadataToken.Name);
            Attributes = MetadataToken.Attributes;
            MethodImplementationFlags = MetadataToken.ImplAttributes;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
            _declaringType = MetadataState.GetLazyCodeElement<TypeDefinition>(MetadataToken.GetDeclaringType());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(MetadataToken.GetDeclarativeSecurityAttributes());
            _genericParameters = MetadataState.GetLazyCodeElements<GenericParameter>(MetadataToken.GetGenericParameters());
            _import = MetadataState.GetLazyCodeElement<MethodImport>(MetadataToken.GetImport());
            _methodBody = new Lazy<MethodBody>(() => MetadataToken.RelativeVirtualAddress == 0 ? null : MetadataState.GetCodeElement<MethodBody>(new CodeElementKey<MethodBody>(MetadataToken.RelativeVirtualAddress)));
            _parameters = new Lazy<IEnumerable<Parameter>>(LoadParameters);
            _debugInformation = new Lazy<MethodDebugInformation>(() => !MetadataState.HasDebugMetadata ? null : MetadataState.GetCodeElement<MethodDebugInformation>(metadataHandle.ToDebugInformationHandle()));
            _signature = new Lazy<MethodSignature<TypeBase>>(() => MetadataToken.DecodeSignature(MetadataState.TypeProvider, new GenericContext(_declaringType.Value.GenericTypeParameters, _genericParameters.Value)));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.Attributes" />
        public MethodAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetCustomAttributes" />
        public override IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinitionHandle.ToDebugInformationHandle" />
        /// <summary>Returns a <see cref="MethodDebugInformation" /> corresponding to this handle.</summary>
        /// <remarks></remarks>
        public MethodDebugInformation DebugInformation => _debugInformation.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetDeclarativeSecurityAttributes" />
        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetDeclaringType" />
        public override TypeBase DeclaringType => _declaringType.Value;

        public override IEnumerable<TypeBase> GenericArguments => _genericParameters.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetImport" />
        public MethodImport Import => _import.Value;

        public bool IsAbstract => Attributes.HasFlag(MethodAttributes.Abstract);

        public bool IsAssembly => Attributes.HasFlag(MethodAttributes.Assembly);

        public bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public bool IsFamily => Attributes.HasFlag(MethodAttributes.Family);

        public bool IsFamilyAndAssembly => Attributes.HasFlag(MethodAttributes.FamANDAssem);

        public bool IsFamilyOrAssembly => Attributes.HasFlag(MethodAttributes.FamORAssem);

        public bool IsFinal => Attributes.HasFlag(MethodAttributes.Final);

        public bool IsHideBySig => Attributes.HasFlag(MethodAttributes.HideBySig);

        public bool IsPrivate => Attributes.HasFlag(MethodAttributes.Private);

        public bool IsPublic => Attributes.HasFlag(MethodAttributes.Public);

        public bool IsSpecialName => Attributes.HasFlag(MethodAttributes.SpecialName);

        public bool IsStatic => Attributes.HasFlag(MethodAttributes.Static);

        public bool IsVirtual => Attributes.HasFlag(MethodAttributes.Virtual);

        public MethodBody MethodBody => _methodBody.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.ImplAttributes" />
        public MethodImplAttributes MethodImplementationFlags { get; }

        public override string Name { get; }

        public override IEnumerable<IParameter> Parameters => _parameters.Value;

        protected MethodSignature<TypeBase> Signature => _signature.Value;

        public Document Document => DebugInformation?.Document;

        public string SourceCode => DebugInformation?.SourceCode;

        private IEnumerable<Parameter> LoadParameters()
        {
            var parameters = MetadataState.GetCodeElements<Parameter>(MetadataToken.GetParameters());
            for (var parameterIndex = 0; parameterIndex < Signature.ParameterTypes.Length; parameterIndex++)
            {
                var parameter = parameters.Single(parameterCheck => parameterCheck.Position == parameterIndex + 1);
                parameter.ParameterType = Signature.ParameterTypes[parameterIndex];
                parameter.Member = this;
            }

            return parameters;
        }
    }
}
