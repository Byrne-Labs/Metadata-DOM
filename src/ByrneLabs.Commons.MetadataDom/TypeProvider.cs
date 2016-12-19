using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace ByrneLabs.Commons.MetadataDom
{
    internal class TypeProvider : ISignatureTypeProvider<TypeBase, GenericContext>, ICustomAttributeTypeProvider<TypeBase>
    {
        private readonly MetadataState _metadataState;
        private readonly SystemType _systemType;

        public TypeProvider(MetadataState metadataState)
        {
            _metadataState = metadataState;
            _systemType = _metadataState.GetCodeElement<SystemType>();
        }

        public TypeBase GetArrayType(TypeBase elementType, ArrayShape shape) => (TypeBase) _metadataState.GetCodeElement(elementType.GetType(), elementType, TypeElementModifiers.Array);

        public TypeBase GetByReferenceType(TypeBase elementType) => (TypeBase) _metadataState.GetCodeElement(elementType.GetType(), elementType, TypeElementModifiers.ByRef);

        public TypeBase GetGenericInstantiation(TypeBase genericType, ImmutableArray<TypeBase> typeArguments) => (TypeBase) _metadataState.GetCodeElement(new CodeElementKey(genericType.GetType(), genericType, typeArguments));

        public TypeBase GetPointerType(TypeBase elementType) => (TypeBase) _metadataState.GetCodeElement(elementType.GetType(), elementType, TypeElementModifiers.Pointer);

        public TypeBase GetSystemType() => _systemType;

        public TypeBase GetTypeFromSerializedName(string name) => _metadataState.GetCodeElement<SerializedType>(name);

        public PrimitiveTypeCode GetUnderlyingEnumType(TypeBase type)
        {
            return PrimitiveTypeCode.Int32;

            throw new NotImplementedException();
        }

        public bool IsSystemType(TypeBase type) => type == _systemType;

        public TypeBase GetFunctionPointerType(MethodSignature<TypeBase> signature) => _metadataState.GetCodeElement<FunctionPointer>(signature);

        public TypeBase GetGenericMethodParameter(GenericContext genericContext, int index) => genericContext.MethodParameters[index];

        public TypeBase GetGenericTypeParameter(GenericContext genericContext, int index) => genericContext.TypeParameters[index];

        public TypeBase GetModifiedType(TypeBase modifier, TypeBase unmodifiedType, bool isRequired) => unmodifiedType;

        public TypeBase GetPinnedType(TypeBase elementType)
        {
            throw new NotImplementedException();
        }

        public TypeBase GetTypeFromSpecification(MetadataReader reader, GenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind) => _metadataState.GetCodeElement<TypeSpecification>(handle);

        public TypeBase GetPrimitiveType(PrimitiveTypeCode typeCode) => _metadataState.GetCodeElement<PrimitiveType>(new CodeElementKey<PrimitiveType>(typeCode));

        public TypeBase GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind) => _metadataState.GetCodeElement<TypeDefinition>(handle);

        public TypeBase GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind) => _metadataState.GetCodeElement<TypeReference>(handle);

        public TypeBase GetSZArrayType(TypeBase elementType) => (TypeBase) _metadataState.GetCodeElement(elementType.GetType(), elementType, TypeElementModifiers.Array);
    }
}
