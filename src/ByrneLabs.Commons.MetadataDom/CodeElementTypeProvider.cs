using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace ByrneLabs.Commons.MetadataDom
{
    internal class CodeElementGenericContext
    {
        public CodeElementGenericContext(IEnumerable<TypeBase> typeParameters, IEnumerable<TypeBase> methodParameters)
        {
            TypeParameters = typeParameters.ToImmutableArray();
            MethodParameters = methodParameters.ToImmutableArray();
        }

        public ImmutableArray<TypeBase> MethodParameters { get; }

        public ImmutableArray<TypeBase> TypeParameters { get; }
    }

    internal class CodeElementSignatureTypeProvider : ISignatureTypeProvider<TypeBase, CodeElementGenericContext>
    {
        private readonly MetadataState _metadataState;

        public CodeElementSignatureTypeProvider(MetadataState metadataState)
        {
            _metadataState = metadataState;
        }

        public TypeBase GetArrayType(TypeBase elementType, ArrayShape shape) => (TypeBase)_metadataState.GetCodeElement(elementType.GetType(), elementType, TypeElementModifiers.Array);

        public TypeBase GetByReferenceType(TypeBase elementType) => (TypeBase)_metadataState.GetCodeElement(elementType.GetType(), elementType, TypeElementModifiers.ByRef);

        public TypeBase GetGenericInstantiation(TypeBase genericType, ImmutableArray<TypeBase> typeArguments) => (TypeBase)_metadataState.GetCodeElement(new CodeElementKey(genericType.GetType(), genericType, typeArguments));

        public TypeBase GetPointerType(TypeBase elementType) => (TypeBase)_metadataState.GetCodeElement(elementType.GetType(), elementType, TypeElementModifiers.Pointer);

        public TypeBase GetFunctionPointerType(MethodSignature<TypeBase> signature)=>_metadataState.GetCodeElement<FunctionPointer>(signature);

        public TypeBase GetGenericMethodParameter(CodeElementGenericContext genericContext, int index) => genericContext.MethodParameters[index];

        public TypeBase GetGenericTypeParameter(CodeElementGenericContext genericContext, int index) => genericContext.TypeParameters[index];

        public TypeBase GetModifiedType(TypeBase modifier, TypeBase unmodifiedType, bool isRequired) => unmodifiedType;

        public TypeBase GetPinnedType(TypeBase elementType)
        {
            throw new NotImplementedException();
        }

        public TypeBase GetTypeFromSpecification(MetadataReader reader, CodeElementGenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind) => _metadataState.GetCodeElement<TypeSpecification>(handle);

        public virtual TypeBase GetPrimitiveType(PrimitiveTypeCode typeCode) => _metadataState.GetCodeElement(new CodeElementKey<PrimitiveType>(typeCode));

        public TypeBase GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind) => _metadataState.GetCodeElement<TypeDefinition>(handle);

        public TypeBase GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind) => _metadataState.GetCodeElement<TypeReference>(handle);

        public TypeBase GetSZArrayType(TypeBase elementType) => (TypeBase)_metadataState.GetCodeElement(elementType.GetType(), elementType, TypeElementModifiers.Array);
    }
}
