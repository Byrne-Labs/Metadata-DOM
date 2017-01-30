using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;

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

        public TypeBase GetArrayType(TypeBase elementType, ArrayShape shape) => _metadataState.GetCodeElement<ShapedArray>(elementType, shape);

        public TypeBase GetByReferenceType(TypeBase elementType) => (TypeBase)_metadataState.GetCodeElement(elementType.GetType(), elementType, TypeElementModifiers.ByRef);

        public TypeBase GetGenericInstantiation(TypeBase genericType, ImmutableArray<TypeBase> typeArguments) => typeArguments.Any(typeArgument => typeArgument == null) ? genericType : (TypeBase)_metadataState.GetCodeElement(new CodeElementKey(genericType.GetType(), genericType, typeArguments));

        public TypeBase GetPointerType(TypeBase elementType) => (TypeBase)_metadataState.GetCodeElement(elementType.GetType(), elementType, TypeElementModifiers.Pointer);

        public TypeBase GetSystemType() => _systemType;

        public TypeBase GetTypeFromSerializedName(string name) => _metadataState.GetCodeElement<SerializedType>(name);

        public PrimitiveTypeCode GetUnderlyingEnumType(TypeBase type)
        {
            throw new NotImplementedException();
        }

        public bool IsSystemType(TypeBase type) => type == _systemType;

        public TypeBase GetFunctionPointerType(MethodSignature<TypeBase> signature) => _metadataState.GetCodeElement<FunctionPointer>(signature);

        public TypeBase GetGenericMethodParameter(GenericContext genericContext, int index) => genericContext.ContextAvailable ? genericContext.MethodParameters[index] : null;

        public TypeBase GetGenericTypeParameter(GenericContext genericContext, int index) => genericContext.ContextAvailable ? genericContext.TypeParameters[index] : null;

        private static readonly string[] _ignoredModifierNames = new string[]
            {
                "System.Runtime.CompilerServices.CallConvCdecl",
                "System.Runtime.CompilerServices.CallConvStdcall",
                "System.Runtime.CompilerServices.CallConvThiscall",
                "System.Runtime.CompilerServices.IsSignUnspecifiedByte",
                "System.Runtime.CompilerServices.IsExplicitlyDereferenced",
                "System.Runtime.CompilerServices.IsImplicitlyDereferenced",
                "System.Runtime.CompilerServices.IsUdtReturn",
                "System.Runtime.InteropServices.GCHandle",
                "System.Security.Permissions.SecurityPermissionAttribute"
            };

        private static readonly Dictionary<string, TypeElementModifiers> _modifierMap = new Dictionary<string, TypeElementModifiers>
        {
            {"Microsoft.VisualC.IsConstModifier", TypeElementModifiers.Constant },
            {"System.Runtime.CompilerServices.IsVolatile", TypeElementModifiers.Volatile },
            {"System.Runtime.CompilerServices.IsBoxed", TypeElementModifiers.Boxed },
            {"System.Runtime.CompilerServices.IsConst", TypeElementModifiers.Constant },
            {"System.Runtime.CompilerServices.IsByValue",TypeElementModifiers.ByValue }
        };

        public TypeBase GetModifiedType(TypeBase modifier, TypeBase unmodifiedType, bool isRequired)
        {
            TypeBase modifiedType;
            if (_ignoredModifierNames.Contains(modifier.FullName))
            {
                modifiedType = unmodifiedType;
            }
            else if (_modifierMap.ContainsKey(modifier.FullName))
            {
                modifiedType = (TypeBase)_metadataState.GetCodeElement(unmodifiedType.GetType(), unmodifiedType, _modifierMap[modifier.FullName]);
            }
            else if (modifier.FullName.Equals("System.Runtime.CompilerServices.IsLong") || modifier.FullName.Equals("Microsoft.VisualC.IsLongModifier"))
            {
                modifiedType = GetPrimitiveType(PrimitiveTypeCode.Int64);
            }
            else
            {
                throw new ArgumentException($"Unknown modifier type {modifier.FullName}", nameof(modifier));
            }
            return modifiedType;
        }

        public TypeBase GetPinnedType(TypeBase elementType)
        {
            throw new NotImplementedException();
        }

        public TypeBase GetTypeFromSpecification(MetadataReader reader, GenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind) => rawTypeKind != 18 && rawTypeKind != 17 ? throw new ArgumentException() : _metadataState.GetCodeElement<TypeSpecification>(handle, genericContext);

        public TypeBase GetPrimitiveType(PrimitiveTypeCode typeCode) => _metadataState.GetCodeElement<PrimitiveType>(new CodeElementKey<PrimitiveType>(typeCode));

        public TypeBase GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind) => rawTypeKind != 18 && rawTypeKind != 17 ? throw new ArgumentException() : _metadataState.GetCodeElement<TypeDefinition>(handle);

        public TypeBase GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind) => rawTypeKind != 0 && rawTypeKind != 18 && rawTypeKind != 17 ? throw new ArgumentException() : _metadataState.GetCodeElement<TypeReference>(handle);

        public TypeBase GetSZArrayType(TypeBase elementType) => (TypeBase)_metadataState.GetCodeElement(elementType.GetType(), elementType, TypeElementModifiers.Array);
    }
}
