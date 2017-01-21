using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition" />
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {TextSignature}")]
    //[PublicAPI]
    public abstract class MethodDefinitionBase : MethodBase<MethodDefinitionBase, MethodDefinitionHandle, System.Reflection.Metadata.MethodDefinition>, IContainsSourceCode
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<MethodDebugInformation> _debugInformation;
        private readonly Lazy<ImmutableArray<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<TypeDefinition> _declaringType;
        private readonly Lazy<MethodImport> _import;
        private readonly Lazy<MethodBody> _methodBody;
        private readonly Lazy<ImmutableArray<IParameter>> _parameters;
        private readonly Lazy<MethodSignature<TypeBase>> _signature;

        internal MethodDefinitionBase(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            Name = AsString(RawMetadata.Name);
            Attributes = RawMetadata.Attributes;
            MethodImplementationFlags = RawMetadata.ImplAttributes;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            _declaringType = MetadataState.GetLazyCodeElement<TypeDefinition>(RawMetadata.GetDeclaringType());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(RawMetadata.GetDeclarativeSecurityAttributes());
            _import = MetadataState.GetLazyCodeElement<MethodImport>(RawMetadata.GetImport());
            _methodBody = new Lazy<MethodBody>(() => RawMetadata.RelativeVirtualAddress == 0 ? null : MetadataState.GetCodeElement<MethodBody>(new CodeElementKey<MethodBody>(RawMetadata.RelativeVirtualAddress)));
            _parameters = new Lazy<ImmutableArray<IParameter>>(LoadParameters);
            _debugInformation = new Lazy<MethodDebugInformation>(() => !MetadataState.HasDebugMetadata ? null : MetadataState.GetCodeElement<MethodDebugInformation>(metadataHandle.ToDebugInformationHandle()));
            _signature = new Lazy<MethodSignature<TypeBase>>(() => RawMetadata.DecodeSignature(MetadataState.TypeProvider, new GenericContext(_declaringType.Value.GenericTypeParameters, GenericTypeParameters)));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.Attributes" />
        public MethodAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetCustomAttributes" />
        public override ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinitionHandle.ToDebugInformationHandle" />
        /// <summary>Returns a <see cref="MethodDebugInformation" /> corresponding to this handle.</summary>
        /// <remarks></remarks>
        public MethodDebugInformation DebugInformation => _debugInformation.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetDeclarativeSecurityAttributes" />
        public ImmutableArray<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetDeclaringType" />
        public override TypeBase DeclaringType => _declaringType.Value;

        public override string FullName => $"{DeclaringType.FullName}.{Name}({string.Join(", ", Parameters.Select(parameter => parameter.ParameterType.FullName))})";

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetImport" />
        public MethodImport Import => _import.Value;

        public bool IsAbstract => Attributes.HasFlag(MethodAttributes.Abstract);

        public bool IsAssembly => Attributes.HasFlag(MethodAttributes.Assembly);

        public bool IsEventAdder => IsSpecialName && Name.StartsWith("add_");

        public bool IsEventRaiser => IsSpecialName && Name.StartsWith("raise_");

        public bool IsEventRemover => IsSpecialName && Name.StartsWith("remove_");

        public bool IsFamily => Attributes.HasFlag(MethodAttributes.Family) && !IsPublic && !IsFamilyOrAssembly;

        public bool IsFamilyAndAssembly => Attributes.HasFlag(MethodAttributes.FamANDAssem) && !IsAssembly && !IsPublic;

        public bool IsFamilyOrAssembly => Attributes.HasFlag(MethodAttributes.FamORAssem) && !IsPublic;

        public bool IsFinal => Attributes.HasFlag(MethodAttributes.Final);

        public bool IsHideBySig => Attributes.HasFlag(MethodAttributes.HideBySig);

        public bool IsPrivate => Attributes.HasFlag(MethodAttributes.Private) && !IsAssembly && !IsFamilyOrAssembly;

        public bool IsPropertyGetter => IsSpecialName && Name.StartsWith("get_");

        public bool IsPropertySetter => IsSpecialName && Name.StartsWith("set_");

        public bool IsPublic => Attributes.HasFlag(MethodAttributes.Public);

        public bool IsSpecialName => Attributes.HasFlag(MethodAttributes.SpecialName);

        public bool IsStatic => Attributes.HasFlag(MethodAttributes.Static);

        public bool IsVirtual => Attributes.HasFlag(MethodAttributes.Virtual);

        public MethodBody MethodBody => _methodBody.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.ImplAttributes" />
        public MethodImplAttributes MethodImplementationFlags { get; }

        public override string Name { get; }

        public override ImmutableArray<IParameter> Parameters => _parameters.Value;

        protected MethodSignature<TypeBase> Signature => _signature.Value;

        public Document Document => DebugInformation?.Document;

        public string SourceCode => DebugInformation?.SourceCode;

        private ImmutableArray<IParameter> LoadParameters()
        {
            var allParameters = MetadataState.GetCodeElements<Parameter>(RawMetadata.GetParameters()).ToList();

            if (DeclaringType.FullName.Contains("MethodSamplesWithNoBounds"))
            {
                var signature = DecodeSignature(MetadataState.TypeProvider, new GenericContext(_declaringType.Value.GenericTypeParameters, GenericTypeParameters));
            }
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
                foreach (var parameter in parameters)
                {
                    parameter.ParameterType = Signature.ParameterTypes[parameter.Position];
                    parameter.Member = this;
                }
            }

            return parameters.Cast<IParameter>().ToImmutableArray();
        }

        public MethodSignature<TType> DecodeSignature<TType, TGenericContext>(ISignatureTypeProvider<TType, TGenericContext> provider, TGenericContext genericContext)
        {
            var decoder = new SignatureDecoder<TType, TGenericContext>(provider, MetadataState.AssemblyReader, genericContext);
            var blobReader = MetadataState.AssemblyReader.GetBlobReader(RawMetadata.Signature);
            return decoder.DecodeMethodSignature(ref blobReader);
        }



        public struct SignatureDecoder<TType, TGenericContext>
        {
            private readonly ISignatureTypeProvider<TType, TGenericContext> _provider;
            private readonly MetadataReader _metadataReaderOpt;
            private readonly TGenericContext _genericContext;

            /// <summary>
            /// Creates a new SignatureDecoder.
            /// </summary>
            /// <param name="provider">The provider used to obtain type symbols as the signature is decoded.</param>
            /// <param name="metadataReader">
            /// The metadata reader from which the signature was obtained. It may be null if the given provider allows it.
            /// </param>
            /// <param name="genericContext">
            /// Additional context needed to resolve generic parameters.
            /// </param>
            public SignatureDecoder(
                ISignatureTypeProvider<TType, TGenericContext> provider,
                MetadataReader metadataReader,
                TGenericContext genericContext)
            {

                _metadataReaderOpt = metadataReader;
                _provider = provider;
                _genericContext = genericContext;
            }

            /// <summary>
            /// Decodes a type embedded in a signature and advances the reader past the type.
            /// </summary>
            /// <param name="blobReader">The blob reader positioned at the leading SignatureTypeCode</param>
            /// <param name="allowTypeSpecifications">Allow a <see cref="TypeSpecificationHandle"/> to follow a (CLASS | VALUETYPE) in the signature.
            /// At present, the only context where that would be valid is in a LocalConstantSig as defined by the Portable PDB specification.
            /// </param>
            /// <returns>The decoded type.</returns>
            /// <exception cref="System.BadImageFormatException">The reader was not positioned at a valid signature type.</exception>
            public TType DecodeType(ref BlobReader blobReader, bool allowTypeSpecifications = false)
            {
                return DecodeType(ref blobReader, allowTypeSpecifications, blobReader.ReadCompressedInteger());
            }

            private TType DecodeType(ref BlobReader blobReader, bool allowTypeSpecifications, int typeCode)
            {
                TType elementType;
                int index;

                switch (typeCode)
                {
                    case (int)SignatureTypeCode.Boolean:
                    case (int)SignatureTypeCode.Char:
                    case (int)SignatureTypeCode.SByte:
                    case (int)SignatureTypeCode.Byte:
                    case (int)SignatureTypeCode.Int16:
                    case (int)SignatureTypeCode.UInt16:
                    case (int)SignatureTypeCode.Int32:
                    case (int)SignatureTypeCode.UInt32:
                    case (int)SignatureTypeCode.Int64:
                    case (int)SignatureTypeCode.UInt64:
                    case (int)SignatureTypeCode.Single:
                    case (int)SignatureTypeCode.Double:
                    case (int)SignatureTypeCode.IntPtr:
                    case (int)SignatureTypeCode.UIntPtr:
                    case (int)SignatureTypeCode.Object:
                    case (int)SignatureTypeCode.String:
                    case (int)SignatureTypeCode.Void:
                    case (int)SignatureTypeCode.TypedReference:
                        return _provider.GetPrimitiveType((PrimitiveTypeCode)typeCode);

                    case (int)SignatureTypeCode.Pointer:
                        elementType = DecodeType(ref blobReader);
                        return _provider.GetPointerType(elementType);

                    case (int)SignatureTypeCode.ByReference:
                        elementType = DecodeType(ref blobReader);
                        return _provider.GetByReferenceType(elementType);

                    case (int)SignatureTypeCode.Pinned:
                        elementType = DecodeType(ref blobReader);
                        return _provider.GetPinnedType(elementType);

                    case (int)SignatureTypeCode.SZArray:
                        elementType = DecodeType(ref blobReader);
                        return _provider.GetSZArrayType(elementType);

                    case (int)SignatureTypeCode.FunctionPointer:
                        MethodSignature<TType> methodSignature = DecodeMethodSignature(ref blobReader);
                        return _provider.GetFunctionPointerType(methodSignature);

                    case (int)SignatureTypeCode.Array:
                        return DecodeArrayType(ref blobReader);

                    case (int)SignatureTypeCode.RequiredModifier:
                        return DecodeModifiedType(ref blobReader, isRequired: true);

                    case (int)SignatureTypeCode.OptionalModifier:
                        return DecodeModifiedType(ref blobReader, isRequired: false);

                    case (int)SignatureTypeCode.GenericTypeInstance:
                        return DecodeGenericTypeInstance(ref blobReader);

                    case (int)SignatureTypeCode.GenericTypeParameter:
                        index = blobReader.ReadCompressedInteger();
                        return _provider.GetGenericTypeParameter(_genericContext, index);

                    case (int)SignatureTypeCode.GenericMethodParameter:
                        index = blobReader.ReadCompressedInteger();
                        return _provider.GetGenericMethodParameter(_genericContext, index);

                    case (int)SignatureTypeKind.Class:
                    case (int)SignatureTypeKind.ValueType:
                        return DecodeTypeHandle(ref blobReader, (byte)typeCode, allowTypeSpecifications);

                    default:
                        throw new BadImageFormatException("SR.Format(SR.UnexpectedSignatureTypeCode, typeCode)");
                }
            }

            /// <summary> 
            /// Decodes a list of types, with at least one instance that is preceded by its count as a compressed integer.
            /// </summary>
            private ImmutableArray<TType> DecodeTypeSequence(ref BlobReader blobReader)
            {
                int count = blobReader.ReadCompressedInteger();
                if (count == 0)
                {
                    // This method is used for Local signatures and method specs, neither of which can have
                    // 0 elements. Parameter sequences can have 0 elements, but they are handled separately
                    // to deal with the sentinel/varargs case.
                    throw new BadImageFormatException("SR.SignatureTypeSequenceMustHaveAtLeastOneElement");
                }

                var types = ImmutableArray.CreateBuilder<TType>(count);

                for (int i = 0; i < count; i++)
                {
                    types.Add(DecodeType(ref blobReader));
                }

                return types.MoveToImmutable();
            }

            /// <summary>
            /// Decodes a method (definition, reference, or standalone) or property signature blob.
            /// </summary>
            /// <param name="blobReader">BlobReader positioned at a method signature.</param>
            /// <returns>The decoded method signature.</returns>
            /// <exception cref="System.BadImageFormatException">The method signature is invalid.</exception>
            public MethodSignature<TType> DecodeMethodSignature(ref BlobReader blobReader)
            {
                SignatureHeader header = blobReader.ReadSignatureHeader();
                CheckMethodOrPropertyHeader(header);

                int genericParameterCount = 0;
                if (header.IsGeneric)
                {
                    genericParameterCount = blobReader.ReadCompressedInteger();
                }

                int parameterCount = blobReader.ReadCompressedInteger();
                TType returnType = DecodeType(ref blobReader);
                ImmutableArray<TType> parameterTypes;
                int requiredParameterCount;

                if (parameterCount == 0)
                {
                    requiredParameterCount = 0;
                    parameterTypes = ImmutableArray<TType>.Empty;
                }
                else
                {
                    var parameterBuilder = ImmutableArray.CreateBuilder<TType>(parameterCount);
                    int parameterIndex;

                    for (parameterIndex = 0; parameterIndex < parameterCount; parameterIndex++)
                    {
                        int typeCode = blobReader.ReadCompressedInteger();
                        if (typeCode == (int)SignatureTypeCode.Sentinel)
                        {
                            break;
                        }
                        parameterBuilder.Add(DecodeType(ref blobReader, allowTypeSpecifications: false, typeCode: typeCode));
                    }

                    requiredParameterCount = parameterIndex;
                    for (; parameterIndex < parameterCount; parameterIndex++)
                    {
                        parameterBuilder.Add(DecodeType(ref blobReader));
                    }
                    parameterTypes = parameterBuilder.MoveToImmutable();
                }

                return new MethodSignature<TType>(header, returnType, requiredParameterCount, genericParameterCount, parameterTypes);
            }

            /// <summary>
            /// Decodes a method specification signature blob and advances the reader past the signature.
            /// </summary>
            /// <param name="blobReader">A BlobReader positioned at a valid method specification signature.</param>
            /// <returns>The types used to instantiate a generic method via the method specification.</returns>
            public ImmutableArray<TType> DecodeMethodSpecificationSignature(ref BlobReader blobReader)
            {
                SignatureHeader header = blobReader.ReadSignatureHeader();
                CheckHeader(header, SignatureKind.MethodSpecification);
                return DecodeTypeSequence(ref blobReader);
            }

            /// <summary>
            /// Decodes a local variable signature blob and advances the reader past the signature.
            /// </summary>
            /// <param name="blobReader">The blob reader positioned at a local variable signature.</param>
            /// <returns>The local variable types.</returns>
            /// <exception cref="System.BadImageFormatException">The local variable signature is invalid.</exception>
            public ImmutableArray<TType> DecodeLocalSignature(ref BlobReader blobReader)
            {
                SignatureHeader header = blobReader.ReadSignatureHeader();
                CheckHeader(header, SignatureKind.LocalVariables);
                return DecodeTypeSequence(ref blobReader);
            }

            /// <summary>
            /// Decodes a field signature blob and advances the reader past the signature.
            /// </summary>
            /// <param name="blobReader">The blob reader positioned at a field signature.</param>
            /// <returns>The decoded field type.</returns>
            public TType DecodeFieldSignature(ref BlobReader blobReader)
            {
                SignatureHeader header = blobReader.ReadSignatureHeader();
                CheckHeader(header, SignatureKind.Field);
                return DecodeType(ref blobReader);
            }

            private TType DecodeArrayType(ref BlobReader blobReader)
            {
                // PERF_TODO: Cache/reuse common case of small number of all-zero lower-bounds.

                TType elementType = DecodeType(ref blobReader);
                int rank = blobReader.ReadCompressedInteger();
                var sizes = ImmutableArray<int>.Empty;
                var lowerBounds = ImmutableArray<int>.Empty;

                int sizesCount = blobReader.ReadCompressedInteger();
                if (sizesCount > 0)
                {
                    var builder = ImmutableArray.CreateBuilder<int>(sizesCount);
                    for (int i = 0; i < sizesCount; i++)
                    {
                        builder.Add(blobReader.ReadCompressedInteger());
                    }
                    sizes = builder.MoveToImmutable();
                }

                int lowerBoundsCount = blobReader.ReadCompressedInteger();
                if (lowerBoundsCount > 0)
                {
                    var builder = ImmutableArray.CreateBuilder<int>(lowerBoundsCount);
                    for (int i = 0; i < lowerBoundsCount; i++)
                    {
                        builder.Add(blobReader.ReadCompressedSignedInteger());
                    }
                    lowerBounds = builder.MoveToImmutable();
                }

                var arrayShape = new ArrayShape(rank, sizes, lowerBounds);
                return _provider.GetArrayType(elementType, arrayShape);
            }

            private TType DecodeGenericTypeInstance(ref BlobReader blobReader)
            {
                TType genericType = DecodeType(ref blobReader);
                ImmutableArray<TType> types = DecodeTypeSequence(ref blobReader);
                return _provider.GetGenericInstantiation(genericType, types);
            }

            private TType DecodeModifiedType(ref BlobReader blobReader, bool isRequired)
            {
                TType modifier = DecodeTypeHandle(ref blobReader, 0, allowTypeSpecifications: true);
                TType unmodifiedType = DecodeType(ref blobReader);

                return _provider.GetModifiedType(modifier, unmodifiedType, isRequired);
            }

            private TType DecodeTypeHandle(ref BlobReader blobReader, byte rawTypeKind, bool allowTypeSpecifications)
            {
                EntityHandle handle = blobReader.ReadTypeHandle();
                if (!handle.IsNil)
                {
                    switch (handle.Kind)
                    {
                        case HandleKind.TypeDefinition:
                            return _provider.GetTypeFromDefinition(_metadataReaderOpt, (TypeDefinitionHandle)handle, rawTypeKind);

                        case HandleKind.TypeReference:
                            return _provider.GetTypeFromReference(_metadataReaderOpt, (TypeReferenceHandle)handle, rawTypeKind);

                        case HandleKind.TypeSpecification:
                            if (!allowTypeSpecifications)
                            {
                                // To prevent cycles, the token following (CLASS | VALUETYPE) must not be a type spec.
                                // https://github.com/dotnet/coreclr/blob/8ff2389204d7c41b17eff0e9536267aea8d6496f/src/md/compiler/mdvalidator.cpp#L6154-L6160
                                throw new BadImageFormatException("SR.NotTypeDefOrRefHandle");
                            }

                            return _provider.GetTypeFromSpecification(_metadataReaderOpt, _genericContext, (TypeSpecificationHandle)handle, rawTypeKind);

                        default:
                            // indicates an error returned from ReadTypeHandle, otherwise unreachable.
                            Debug.Assert(handle.IsNil); // will fall through to throw in release.
                            break;
                    }
                }

                throw new BadImageFormatException("SR.NotTypeDefOrRefOrSpecHandle");
            }

            private void CheckHeader(SignatureHeader header, SignatureKind expectedKind)
            {
                if (header.Kind != expectedKind)
                {
                    throw new BadImageFormatException("SR.Format(SR.UnexpectedSignatureHeader, expectedKind, header.Kind, header.RawValue)");
                }
            }

            private void CheckMethodOrPropertyHeader(SignatureHeader header)
            {
                SignatureKind kind = header.Kind;
                if (kind != SignatureKind.Method && kind != SignatureKind.Property)
                {
                    throw new BadImageFormatException("SR.Format(SR.UnexpectedSignatureHeader2, SignatureKind.Property, SignatureKind.Method, header.Kind, header.RawValue)");
                }
            }
        }

    }
}
