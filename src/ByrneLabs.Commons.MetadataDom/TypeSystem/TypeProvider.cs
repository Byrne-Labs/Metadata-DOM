using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    internal class TypeProvider : ISignatureTypeProvider<TypeBase, GenericContext>, ICustomAttributeTypeProvider<TypeBase>
    {
        private static readonly string[] _ignoredModifierNames =
        {
            "Microsoft.VisualC.IsLongModifier",
            "System.Runtime.CompilerServices.CallConvCdecl",
            "System.Runtime.CompilerServices.CallConvStdcall",
            "System.Runtime.CompilerServices.CallConvThiscall",
            "System.Runtime.CompilerServices.IsCopyConstructed",
            "System.Runtime.CompilerServices.IsSignUnspecifiedByte",
            "System.Runtime.CompilerServices.IsExplicitlyDereferenced",
            "System.Runtime.CompilerServices.IsImplicitlyDereferenced",
            "System.Runtime.CompilerServices.IsJitIntrinsic",
            "System.Runtime.CompilerServices.IsLong",
            "System.Runtime.CompilerServices.IsUdtReturn",
            "System.Runtime.InteropServices.GCHandle",
            "System.Security.Permissions.SecurityPermissionAttribute"
        };
        private static readonly Dictionary<string, TypeElementModifier> _modifierMap = new Dictionary<string, TypeElementModifier>
        {
            { "Microsoft.VisualC.IsConstModifier", TypeElementModifier.Constant },
            { "System.Runtime.CompilerServices.IsVolatile", TypeElementModifier.Volatile },
            { "System.Runtime.CompilerServices.IsBoxed", TypeElementModifier.Boxed },
            { "System.Runtime.CompilerServices.IsConst", TypeElementModifier.Constant },
            { "System.Runtime.CompilerServices.IsByValue", TypeElementModifier.ByValue },
            { "System.ValueType", TypeElementModifier.ValueType }
        };
        private readonly MetadataState _metadataState;
        private readonly SystemType _systemType;

        public TypeProvider(MetadataState metadataState)
        {
            _metadataState = metadataState;
            _systemType = _metadataState.GetCodeElement<SystemType>();
        }

        public TypeBase GetArrayType(TypeBase elementType, ArrayShape shape) => _metadataState.GetCodeElement<ShapedArray>(elementType, shape);

        public TypeBase GetByReferenceType(TypeBase elementType) => (TypeBase) _metadataState.GetCodeElement(elementType.GetType(), elementType, TypeElementModifier.ByRef);

        public TypeBase GetFunctionPointerType(MethodSignature<TypeBase> signature) => _metadataState.GetCodeElement<FunctionPointer>(signature);

        public TypeBase GetGenericInstantiation(TypeBase genericType, ImmutableArray<TypeBase> typeArguments) => typeArguments.Any(typeArgument => typeArgument == null) ? genericType : (TypeBase) _metadataState.GetCodeElement(new CodeElementKey(genericType.GetType(), genericType, typeArguments));

        public TypeBase GetGenericMethodParameter(GenericContext genericContext, int index) => (genericContext.ContextAvailable && genericContext.MethodParameters.Length > index ? genericContext.MethodParameters[index] : _metadataState.GetCodeElement<GenericParameterPlaceholder>(genericContext.RequestingCodeElement, index)) as TypeBase;

        public TypeBase GetGenericTypeParameter(GenericContext genericContext, int index) => (genericContext.ContextAvailable && genericContext.TypeParameters.Length > index ? genericContext.TypeParameters[index] : _metadataState.GetCodeElement<GenericParameterPlaceholder>(genericContext.RequestingCodeElement, index)) as TypeBase;

        public TypeBase GetModifiedType(TypeBase modifier, TypeBase unmodifiedType, bool isRequired)
        {
            TypeBase modifiedType;
            if (_ignoredModifierNames.Contains(modifier.FullName))
            {
                modifiedType = unmodifiedType;
            }
            else if (_modifierMap.ContainsKey(modifier.FullName))
            {
                modifiedType = (TypeBase) _metadataState.GetCodeElement(unmodifiedType.GetType(), unmodifiedType, _modifierMap[modifier.FullName]);
            }
            else
            {
                throw new ArgumentException($"Unknown modifier type {modifier.FullName}", nameof(modifier));
            }

            return modifiedType;
        }

        public TypeBase GetPinnedType(TypeBase elementType) => throw new NotSupportedException("This will be supported in the future");

        public TypeBase GetPointerType(TypeBase elementType) => (TypeBase) _metadataState.GetCodeElement(elementType.GetType(), elementType, TypeElementModifier.Pointer);

        public TypeBase GetPrimitiveType(PrimitiveTypeCode typeCode) => _metadataState.GetCodeElement<PrimitiveType>(new CodeElementKey<PrimitiveType>(typeCode));

        public TypeBase GetSystemType() => _systemType;

        public TypeBase GetSZArrayType(TypeBase elementType) => (TypeBase) _metadataState.GetCodeElement(elementType.GetType(), elementType, TypeElementModifier.Array);

        public TypeBase GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind) => rawTypeKind != 18 && rawTypeKind != 17 ? throw new ArgumentException() : _metadataState.GetCodeElement<TypeDefinition>(handle);

        public TypeBase GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind) => rawTypeKind != 0 && rawTypeKind != 18 && rawTypeKind != 17 ? throw new ArgumentException() : _metadataState.GetCodeElement<TypeReference>(handle);

        public TypeBase GetTypeFromSerializedName(string name) => _metadataState.GetCodeElement<SerializedType>(name);

        public TypeBase GetTypeFromSpecification(MetadataReader reader, GenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind) => rawTypeKind != 18 && rawTypeKind != 17 ? throw new ArgumentException() : _metadataState.GetCodeElement<TypeSpecification>(handle, genericContext);

        public PrimitiveTypeCode GetUnderlyingEnumType(TypeBase type) => throw new NotSupportedException("This will be supported in the future");

        public bool IsSystemType(TypeBase type) => type == _systemType;
    }
}
