// Note: this class is based on https://github.com/dotnet/corefx/blob/master/src/System.Reflection.Metadata/tests/Metadata/Decoding/DisassemblingTypeProvider.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace ByrneLabs.Commons.MetadataDom
{
    internal class DisassemblingGenericContext
    {
        public DisassemblingGenericContext(IEnumerable<string> typeParameters, IEnumerable<string> methodParameters)
        {
            TypeParameters = typeParameters.ToImmutableArray();
            MethodParameters = methodParameters.ToImmutableArray();
        }

        public ImmutableArray<string> MethodParameters { get; }

        public ImmutableArray<string> TypeParameters { get; }
    }

    internal class DisassemblingTypeProvider : ISignatureTypeProvider<string, DisassemblingGenericContext>
    {
        public virtual string GetArrayType(string elementType, ArrayShape shape)
        {
            var builder = new StringBuilder();

            builder.Append(elementType);
            builder.Append('[');

            for (var i = 0; i < shape.Rank; i++)
            {
                var lowerBound = 0;

                if (i < shape.LowerBounds.Length)
                {
                    lowerBound = shape.LowerBounds[i];
                    builder.Append(lowerBound);
                }

                builder.Append("...");

                if (i < shape.Sizes.Length)
                {
                    builder.Append(lowerBound + shape.Sizes[i] - 1);
                }

                if (i < shape.Rank - 1)
                {
                    builder.Append(',');
                }
            }

            builder.Append(']');
            return builder.ToString();
        }

        public virtual string GetByReferenceType(string elementType) => elementType + "&";

        public virtual string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments) => genericType + "<" + string.Join(",", typeArguments) + ">";

        public virtual string GetPointerType(string elementType) => elementType + "*";

        public virtual string GetFunctionPointerType(MethodSignature<string> signature)
        {
            var parameterTypes = signature.ParameterTypes;

            var requiredParameterCount = signature.RequiredParameterCount;

            var builder = new StringBuilder();
            builder.Append("method ");
            builder.Append(signature.ReturnType);
            builder.Append(" *(");

            int i;
            for (i = 0; i < requiredParameterCount; i++)
            {
                builder.Append(parameterTypes[i]);
                if (i < parameterTypes.Length - 1)
                {
                    builder.Append(", ");
                }
            }

            if (i < parameterTypes.Length)
            {
                builder.Append("..., ");
                for (; i < parameterTypes.Length; i++)
                {
                    builder.Append(parameterTypes[i]);
                    if (i < parameterTypes.Length - 1)
                    {
                        builder.Append(", ");
                    }
                }
            }

            builder.Append(')');
            return builder.ToString();
        }

        public virtual string GetGenericMethodParameter(DisassemblingGenericContext genericContext, int index) => "!!" + genericContext.MethodParameters[index];

        public virtual string GetGenericTypeParameter(DisassemblingGenericContext genericContext, int index) => "!" + genericContext.TypeParameters[index];

        public virtual string GetModifiedType(string modifierType, string unmodifiedType, bool isRequired) => unmodifiedType + (isRequired ? " modreq(" : " modopt(") + modifierType + ")";

        public virtual string GetPinnedType(string elementType) => elementType + " pinned";

        public virtual string GetTypeFromSpecification(MetadataReader reader, DisassemblingGenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind = 0) => reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);

        public virtual string GetPrimitiveType(PrimitiveTypeCode typeCode)
        {
            switch (typeCode)
            {
                case PrimitiveTypeCode.Boolean:
                    return "bool";

                case PrimitiveTypeCode.Byte:
                    return "uint8";

                case PrimitiveTypeCode.Char:
                    return "char";

                case PrimitiveTypeCode.Double:
                    return "float64";

                case PrimitiveTypeCode.Int16:
                    return "int16";

                case PrimitiveTypeCode.Int32:
                    return "int32";

                case PrimitiveTypeCode.Int64:
                    return "int64";

                case PrimitiveTypeCode.IntPtr:
                    return "native int";

                case PrimitiveTypeCode.Object:
                    return "object";

                case PrimitiveTypeCode.SByte:
                    return "int8";

                case PrimitiveTypeCode.Single:
                    return "float32";

                case PrimitiveTypeCode.String:
                    return "string";

                case PrimitiveTypeCode.TypedReference:
                    return "typedref";

                case PrimitiveTypeCode.UInt16:
                    return "uint16";

                case PrimitiveTypeCode.UInt32:
                    return "uint32";

                case PrimitiveTypeCode.UInt64:
                    return "uint64";

                case PrimitiveTypeCode.UIntPtr:
                    return "native uint";

                case PrimitiveTypeCode.Void:
                    return "void";

                default:
                    throw new ArgumentOutOfRangeException(nameof(typeCode));
            }
        }

        public virtual string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind = 0)
        {
            var definition = reader.GetTypeDefinition(handle);

            var name = definition.Namespace.IsNil
                ? reader.GetString(definition.Name)
                : reader.GetString(definition.Namespace) + "." + reader.GetString(definition.Name);

            if ((definition.Attributes & (TypeAttributes.NestedAssembly | TypeAttributes.NestedFamily | TypeAttributes.NestedFamANDAssem | TypeAttributes.NestedFamORAssem | TypeAttributes.NestedPrivate | TypeAttributes.NestedPublic)) != 0)
            {
                var declaringTypeHandle = definition.GetDeclaringType();
                return GetTypeFromDefinition(reader, declaringTypeHandle, 0) + "/" + name;
            }

            return name;
        }

        public virtual string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind = 0)
        {
            var reference = reader.GetTypeReference(handle);
            Handle scope = reference.ResolutionScope;

            var name = reference.Namespace.IsNil
                ? reader.GetString(reference.Name)
                : reader.GetString(reference.Namespace) + "." + reader.GetString(reference.Name);

            switch (scope.Kind)
            {
                case HandleKind.ModuleReference:
                    return "[.module  " + reader.GetString(reader.GetModuleReference((ModuleReferenceHandle)scope).Name) + "]" + name;

                case HandleKind.AssemblyReference:
                    var assemblyReferenceHandle = (AssemblyReferenceHandle)scope;
                    var assemblyReference = reader.GetAssemblyReference(assemblyReferenceHandle);
                    return "[" + reader.GetString(assemblyReference.Name) + "]" + name;

                case HandleKind.TypeReference:
                    return GetTypeFromReference(reader, (TypeReferenceHandle)scope) + "/" + name;

                default:
                    // rare cases:  ModuleDefinition means search within defs of current module (used by WinMDs for projections)
                    //              nil means search exported types of same module (haven't seen this in practice). For the test
                    //              purposes here, it's sufficient to format both like defs.
                    Debug.Assert(scope == Handle.ModuleDefinition || scope.IsNil);
                    return name;
            }
        }

        public virtual string GetSZArrayType(string elementType) => elementType + "[]";

        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases", Justification = "Other handle types will not have a signature to decode")]
        public virtual string GetTypeFromHandle(MetadataReader reader, DisassemblingGenericContext genericContext, EntityHandle handle)
        {
            switch (handle.Kind)
            {
                case HandleKind.TypeDefinition:
                    return GetTypeFromDefinition(reader, (TypeDefinitionHandle)handle);

                case HandleKind.TypeReference:
                    return GetTypeFromReference(reader, (TypeReferenceHandle)handle);

                case HandleKind.TypeSpecification:
                    return GetTypeFromSpecification(reader, genericContext, (TypeSpecificationHandle)handle);

                default:
                    throw new ArgumentOutOfRangeException(nameof(handle));
            }
        }
    }
}


