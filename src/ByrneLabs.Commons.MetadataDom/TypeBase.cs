using System;
using System.Diagnostics;
using JetBrains.Annotations;
using System.Reflection.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{

    public abstract class TypeBase<TTypeBase, THandle, TToken> : TypeBase<TTypeBase, THandle>, ICodeElementWithHandle<THandle, TToken> where TTypeBase : TypeBase
    {
        internal TypeBase(TypeBase<TTypeBase, THandle> baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState)
        {
            DowncastMetadataHandle = MetadataState.DowncastHandle(MetadataHandle).Value;
            MetadataToken = (TToken)MetadataState.GetTokenForHandle(MetadataHandle);
        }

        internal TypeBase(TypeBase<TTypeBase, THandle> genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
            DowncastMetadataHandle = MetadataState.DowncastHandle(MetadataHandle).Value;
            MetadataToken = (TToken)MetadataState.GetTokenForHandle(MetadataHandle);
        }

        internal TypeBase(THandle handle, MetadataState metadataState) : base(handle, metadataState)
        {
            DowncastMetadataHandle = MetadataState.DowncastHandle(MetadataHandle).Value;
            MetadataToken = (TToken)MetadataState.GetTokenForHandle(MetadataHandle);
        }

        public Handle DowncastMetadataHandle { get; }

        public THandle MetadataHandle => KeyValue;

        public TToken MetadataToken { get; }
    }

    public abstract class TypeBase<TTypeBase, TKey> : TypeBase where TTypeBase : TypeBase
    {
        internal TypeBase(TypeBase<TTypeBase, TKey> baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState, new CodeElementKey<TTypeBase>(baseType, typeElementModifiers))
        {
            KeyValue = baseType.KeyValue;
        }

        internal TypeBase(TypeBase<TTypeBase, TKey> genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState, new CodeElementKey<TTypeBase>(genericTypeDefinition, genericTypeDefinition, genericTypeArguments))
        {
            KeyValue = genericTypeDefinition.KeyValue;
        }

        internal TypeBase(TKey keyValue, MetadataState metadataState) : base(new CodeElementKey<TTypeBase>(keyValue), metadataState)
        {
            KeyValue = keyValue;
        }

        protected TKey KeyValue { get; }
    }

    [DebuggerDisplay("\\{{GetType().FullName,nq}\\}: {Namespace,nq}.{Name,nq}")]
    [PublicAPI]
    public abstract class TypeBase : RuntimeCodeElement
    {
        internal TypeBase(TypeBase baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState, CodeElementKey key) : this(key, metadataState)
        {
            IsArray = (typeElementModifiers.HasFlag(TypeElementModifiers.Array));
            IsByRef = (typeElementModifiers.HasFlag(TypeElementModifiers.ByRef));
            IsGenericType = (typeElementModifiers.HasFlag(TypeElementModifiers.Generic));
            IsPointer = (typeElementModifiers.HasFlag(TypeElementModifiers.Pointer));

            if (IsArray || IsPointer)
            {
                ElementType = baseType;
            }
        }

        internal TypeBase(TypeBase genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState, CodeElementKey key) : this(key, metadataState)
        {
            IsGenericType = true;
            GenericTypeDefinition = genericTypeDefinition;
            GenericTypeArguments = genericTypeArguments.ToArray();
        }

        internal TypeBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        /// <inheritdoc cref="System.Type.AssemblyQualifiedName" />
        public string AssemblyQualifiedName { get; protected set; }

        /// <summary>When overridden in a derived class, returns the <see cref="TypeBase" /> of the object encompassed or referred to by the current array, pointer or reference type.</summary>
        /// <returns>The <see cref="TypeBase" /> of the object encompassed or referred to by the current array, pointer, or reference type, or null if the current
        ///     <see cref="TypeBase" /> is not an array or a pointer, or is not passed by reference, or represents a generic type or a type parameter in the definition of a generic type or generic method.</returns>
        public TypeBase ElementType { get; protected set; }

        /// <inheritdoc cref="System.Type.FullName" />
        public string FullName { get; protected set; }

        public TypeBase[] GenericTypeArguments { get; protected set; }

        public bool IsAbstract { get; protected set; }

        public bool IsArray { get; protected set; }

        public bool IsByRef { get; protected set; }

        public bool IsClass { get; protected set; }

        /// <inheritdoc cref="System.Reflection.TypeInfo.IsEnum" />
        public bool IsEnum { get; protected set; }

        public bool IsGenericParameter { get; protected set; }

        public bool IsGenericType { get; protected set; }

        public bool IsGenericTypeDefinition { get; protected set; }

        public bool IsImport { get; protected set; }

        public bool IsInterface { get; protected set; }

        /// <inheritdoc cref="System.Type.IsNested" />
        public bool IsNested => IsNestedAssembly || IsNestedFamily || IsNestedFamANDAssem || IsNestedFamORAssem || IsNestedPrivate || IsNestedPublic;

        public bool IsNestedAssembly { get; protected set; }

        public bool IsNestedFamANDAssem { get; protected set; }

        public bool IsNestedFamily { get; protected set; }

        public bool IsNestedFamORAssem { get; protected set; }

        public bool IsNestedPrivate { get; protected set; }

        public bool IsNestedPublic { get; protected set; }

        public bool IsNotPublic { get; protected set; }

        public bool IsPointer { get; protected set; }

        public bool IsPrimitive { get; protected set; }

        public bool IsPublic { get; protected set; }

        public string Name { get; protected set; }

        public string Namespace { get; protected set; }

        protected TypeBase GenericTypeDefinition { get; set; }

        /// <summary>Returns a <see cref="TypeDefinition" /> object that represents a generic type definition from which the current generic type can be constructed.</summary>
        /// <returns>A <see cref="TypeDefinition" /> object representing a generic type from which the current type can be constructed.</returns>
        /// <exception cref="System.InvalidOperationException">The current type is not a generic type.  That is, <see cref="IsGenericType" /> returns false. </exception>
        public TypeBase GetGenericTypeDefinition()
        {
            if (!IsGenericType)
            {
                throw new InvalidOperationException($"{GetType().FullName} is not a generic type");
            }

            return GenericTypeDefinition;
        }
    }
}
