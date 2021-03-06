﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {FullName}")]
    public class ConstructorDefinition : ConstructorInfo, IManagedCodeElement
    {
        private readonly Lazy<IEnumerable<CustomAttributeData>> _customAttributes;
        private readonly Lazy<MethodDebugInformation> _debugInformation;
        private readonly Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<TypeDefinition> _declaringType;
        private readonly Lazy<string> _fullTextSignature;
        private readonly Lazy<IEnumerable<GenericParameter>> _genericParameters;
        private readonly Lazy<MethodImport> _import;
        private readonly Lazy<MethodBody> _methodBody;
        private readonly Lazy<IEnumerable<ParameterInfo>> _parameters;
        private readonly Lazy<MethodSignature<TypeBase>> _signature;
        private readonly Lazy<string> _textSignature;

        internal ConstructorDefinition(MethodDefinitionHandle metadataHandle, MetadataState metadataState)
        {
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            Key = new CodeElementKey<ConstructorDefinition>(metadataHandle);
            RawMetadata = MetadataState.AssemblyReader.GetMethodDefinition(metadataHandle);
            _fullTextSignature = new Lazy<string>(() => $"{DeclaringType.FullName}.{TextSignature}");
            _textSignature = new Lazy<string>(() =>
            {
                var genericParameters = GenericTypeParameters.Any() ? $"<{string.Join(", ", GenericTypeParameters.Select(genericTypeParameter => genericTypeParameter.Name))}>" : string.Empty;
                var parameters = $"({string.Join(", ", GetParameters().Select(parameter => parameter.ParameterType.IsGenericParameter ? parameter.ParameterType.Name : ((TypeBase) parameter.ParameterType).FullNameWithoutAssemblies))})";

                return Name.Substring(1) + genericParameters + parameters;
            });
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            _genericParameters = new Lazy<IEnumerable<GenericParameter>>(() =>
            {
                IEnumerable<GenericParameter> genericParameters;
                if (_declaringType.Value.IsDelegate)
                {
                    genericParameters = _declaringType.Value.GenericTypeParameters.Cast<GenericParameter>();
                }
                else
                {
                    genericParameters = MetadataState.GetCodeElements<GenericParameter>(RawMetadata.GetGenericParameters());
                    foreach (var genericParameter in genericParameters)
                    {
                        genericParameter.SetDeclaringMethod(this);
                        genericParameter.SetDeclaringType((TypeBase) DeclaringType);
                    }
                }

                return genericParameters;
            });

            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttributeData>(RawMetadata.GetCustomAttributes());
            _declaringType = MetadataState.GetLazyCodeElement<TypeDefinition>(RawMetadata.GetDeclaringType());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(RawMetadata.GetDeclarativeSecurityAttributes());
            _import = MetadataState.GetLazyCodeElement<MethodImport>(RawMetadata.GetImport());
            _methodBody = new Lazy<MethodBody>(() => RawMetadata.RelativeVirtualAddress == 0 ? null : MetadataState.GetCodeElement<MethodBody>(new CodeElementKey<MethodBody>(RawMetadata.RelativeVirtualAddress)));
            _parameters = new Lazy<IEnumerable<ParameterInfo>>(LoadParameters);
            _debugInformation = new Lazy<MethodDebugInformation>(() => !MetadataState.HasDebugMetadata ? null : MetadataState.GetCodeElement<MethodDebugInformation>(metadataHandle.ToDebugInformationHandle(), this, new GenericContext(this, _declaringType.Value.GenericTypeParameters, null)));
            _signature = new Lazy<MethodSignature<TypeBase>>(() => RawMetadata.DecodeSignature(MetadataState.TypeProvider, new GenericContext(this, _declaringType.Value.GenericTypeParameters, GenericTypeParameters)));
        }

        public override MethodAttributes Attributes => RawMetadata.Attributes;

        public override CallingConventions CallingConvention => Signature.Header.CallingConvention == SignatureCallingConvention.Default ? CallingConventions.Standard | CallingConventions.HasThis : throw new ArgumentException($"Unable to handle the signature calling convention {Signature.Header.CallingConvention}");

        public override bool ContainsGenericParameters => _genericParameters.Value.Any() || DeclaringType.ContainsGenericParameters;

        public override MetadataDom.MethodDebugInformation DebugInformation => _debugInformation.Value;

        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public override Type DeclaringType => _declaringType.Value;

        public override string FullName => FullTextSignature;

        public override string FullTextSignature => _fullTextSignature.Value;

        public IEnumerable<TypeInfo> GenericTypeParameters { get; } = ImmutableArray<TypeInfo>.Empty;

        public MethodImport Import => _import.Value;

        public override bool IsGenericMethod => false;

        public override bool IsGenericMethodDefinition => false;

        public override MemberTypes MemberType => MemberTypes.Constructor;

        public MethodDefinitionHandle MetadataHandle { get; }

        public override int MetadataToken => Key.Handle.Value.GetHashCode();

        public override RuntimeMethodHandle MethodHandle => throw NotSupportedHelper.NotValidForMetadata();

        public override System.Reflection.Module Module => MetadataState.ModuleDefinition;

        public override string Name { get; }

        public override IEnumerable<System.Reflection.ParameterInfo> Parameters => _parameters.Value.ToImmutableList();

        public System.Reflection.Metadata.MethodDefinition RawMetadata { get; }

        public override Type ReflectedType => null;

        public override IEnumerable<MetadataDom.SequencePoint> SequencePoints => DebugInformation?.SequencePoints?.ToImmutableArray();

        public override string SourceCode => DebugInformation?.SourceCode;

        public override string TextSignature => _textSignature.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        internal MethodSignature<TypeBase> Signature => _signature.Value;

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override object[] GetCustomAttributes(bool inherit) => throw NotSupportedHelper.FutureVersion();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();

        public override IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<System.Reflection.CustomAttributeData>();

        public override Type[] GetGenericArguments() => _genericParameters.Value.ToArray<Type>();

        public override System.Reflection.MethodBody GetMethodBody() => _methodBody.Value;

        public override MethodImplAttributes GetMethodImplementationFlags() => RawMetadata.ImplAttributes;

        public override System.Reflection.ParameterInfo[] GetParameters() => _parameters.Value.ToArray<System.Reflection.ParameterInfo>();

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw NotSupportedHelper.NotValidForMetadata();

        public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw NotSupportedHelper.NotValidForMetadata();

        public override bool IsDefined(Type attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();

        private IEnumerable<ParameterInfo> LoadParameters()
        {
            var allParameters = RawMetadata.GetParameters().Select(parameterMetadata => MetadataState.GetCodeElement<Parameter>(parameterMetadata, this)).ToList();
            var parameters = allParameters.Where(parameter => parameter.Position >= 0).ToList();
            if (Signature.ParameterTypes.Any() && parameters.Count == 0)
            {
                /* Parameters do not have to be named in IL.  When this happens, the parameter does not show up in the parameter list but will have a parameter type.  If there are no parameters listed,
                 * we can use the order of the parameter types to get the position. -- Jonathan Byrne 01/11/2017
                */
                parameters.AddRange(Signature.ParameterTypes.Select((parameterType, position) => new Parameter(this, parameterType, position, position > Signature.RequiredParameterCount, MetadataState)));
            }
            else
            {
                if (Signature.ParameterTypes.Length != parameters.Count)
                {
                    throw new BadMetadataException($"Method {DeclaringType.FullName}.{Name} has {parameters.Count} parameters but {Signature.ParameterTypes.Length} parameter types were found");
                }

                for (var position = 0; position < parameters.Count; position++)
                {
                    if (!parameters.Exists(parameter => parameter.Position == position))
                    {
                        throw new BadMetadataException($"Method {DeclaringType.FullName}{Name} has {parameters.Count} parameters but does not have a parameter for position {position}");
                    }
                }
            }

            return parameters.Cast<ParameterInfo>().ToImmutableArray();
        }
    }
}
