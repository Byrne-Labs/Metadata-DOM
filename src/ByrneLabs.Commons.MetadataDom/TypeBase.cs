using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class TypeBase<TTypeBase, THandle, TToken> : TypeBase<TTypeBase, THandle>, ICodeElementWithTypedHandle<THandle, TToken> where TTypeBase : TypeBase
    {
        internal TypeBase(TypeBase<TTypeBase, THandle> baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState)
        {
            RawMetadata = (TToken)MetadataState.GetRawMetadataForHandle(baseType.DowncastMetadataHandle);
        }

        internal TypeBase(TypeBase<TTypeBase, THandle> genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
            RawMetadata = (TToken)MetadataState.GetRawMetadataForHandle(genericTypeDefinition.DowncastMetadataHandle);
        }

        internal TypeBase(THandle handle, MetadataState metadataState) : base(handle, metadataState)
        {
            RawMetadata = (TToken)MetadataState.GetRawMetadataForHandle(handle);
        }

        internal TypeBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
            RawMetadata = (TToken)MetadataState.GetRawMetadataForHandle(key.Handle.Value);
        }

        public TToken RawMetadata { get; }

        public virtual THandle MetadataHandle => KeyValue;
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

        internal TypeBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        protected TKey KeyValue { get; }
    }

    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {FullNameWithoutAssemblies}")]
    //[PublicAPI]
    public abstract class TypeBase : MemberBase
    {
        private Lazy<string> _fullName;
        private Lazy<string> _fullNameWithoutAssemblies;
        private Lazy<string> _fullNameWithoutGenericArguments;
        private Lazy<string> _name;
        private Lazy<string> _nameModifiers;

        internal TypeBase(TypeBase baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState, CodeElementKey key) : this(key, metadataState)
        {
            BaseType = baseType;
            IsThisArray = typeElementModifiers.HasFlag(TypeElementModifiers.Array);
            IsThisByRef = typeElementModifiers.HasFlag(TypeElementModifiers.ByRef);
            IsThisGenericType = typeElementModifiers.HasFlag(TypeElementModifiers.GenericType);
            IsThisPointer = typeElementModifiers.HasFlag(TypeElementModifiers.Pointer);
            IsThisVolatile = typeElementModifiers.HasFlag(TypeElementModifiers.Volatile);
            IsThisConstant = typeElementModifiers.HasFlag(TypeElementModifiers.Constant);
            if (IsArray || IsPointer)
            {
                ElementType = baseType;
            }
            if (IsArray)
            {
                ArrayRank = 1;
            }
            GenericTypeArguments = ImmutableArray<TypeBase>.Empty;
            Initialize();
        }

        internal TypeBase(TypeBase genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState, CodeElementKey key) : this(key, metadataState)
        {
            if (genericTypeArguments.Any(x => x == null))
            {
                throw new ArgumentException();
            }

            IsThisGenericType = true;
            GenericTypeDefinition = genericTypeDefinition;
            GenericTypeArguments = genericTypeArguments.ToImmutableArray();
            Initialize();
        }

        internal TypeBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
            GenericTypeArguments = ImmutableArray<TypeBase>.Empty;
            Initialize();
        }

        public abstract IAssembly Assembly { get; }

        public abstract bool IsGenericParameter { get; }

        public abstract string Namespace { get; }

        internal abstract string UndecoratedName { get; }

        public int ArrayRank { get; protected set; }

        public virtual string AssemblyQualifiedName => Assembly == null ? FullName : $"{FullName}, {Assembly.Name.FullName}";

        /// <summary>When overridden in a derived class, returns the <see cref="TypeBase" /> of the object encompassed or referred to by the current array, pointer or reference type.</summary>
        /// <returns>The <see cref="TypeBase" /> of the object encompassed or referred to by the current array, pointer, or reference type, or null if the current
        ///     <see cref="TypeBase" /> is not an array or a pointer, or is not passed by reference, or represents a generic type or a type parameter in the definition of a generic type or generic method.</returns>
        public TypeBase ElementType { get; protected set; }

        public override string FullName => _fullName.Value;

        public virtual string FullNameWithoutAssemblies => _fullNameWithoutAssemblies.Value;

        public ImmutableArray<TypeBase> GenericTypeArguments { get; }

        public TypeBase GenericTypeDefinition { get; }

        public bool HasElementType => ElementType != null;

        public bool HasGenericTypeArguments => GenericTypeArguments.Any();

        public bool IsArray => BaseType?.IsArray == true || IsThisArray;

        public bool IsByRef => BaseType?.IsByRef == true || IsThisByRef;

        public virtual bool IsGenericType => BaseType?.IsGenericType == true || IsThisGenericType;

        public bool IsNested => DeclaringType != null;

        public bool IsPointer => BaseType?.IsPointer == true || IsThisPointer;

        public bool IsVolatile => BaseType?.IsVolatile == true || IsThisVolatile;
        public bool IsConstant => BaseType?.IsConstant == true || IsThisConstant;

        public override string Name => _name.Value;

        public override string TextSignature => FullName;

        protected int ArrayDimensionCount => (IsThisArray ? 1 : 0) + (BaseType?.ArrayDimensionCount).GetValueOrDefault();

        protected bool IsThisArray { get; }

        protected bool IsThisByRef { get; }

        protected bool IsThisGenericType { get; }

        protected bool IsThisPointer { get; }

        protected bool IsThisVolatile { get; }

        protected bool IsThisConstant { get; }

        protected int PointerCount => (IsThisPointer ? 1 : 0) + (BaseType?.PointerCount).GetValueOrDefault();

        internal virtual TypeBase BaseType { get; }

        internal virtual string FullNameWithoutGenericArguments => _fullNameWithoutGenericArguments.Value;

        private void Initialize()
        {
            _nameModifiers = new Lazy<string>(() => string.Concat(Enumerable.Repeat("[]", ArrayDimensionCount)) + new string('*', PointerCount) + (IsByRef ? "&" : string.Empty));
            _fullName = new Lazy<string>(() =>
            {
                var parent = IsNested && !IsGenericParameter ? DeclaringType.FullName + "+" : (string.IsNullOrEmpty(Namespace) ? string.Empty : Namespace + ".");
                var fullName = parent + Name;

                return fullName;
            });

            _fullNameWithoutAssemblies = new Lazy<string>(() =>
            {
                string parent;
                if (IsGenericParameter && DeclaringType != null || IsNested)
                {
                    parent = $"{DeclaringType.FullNameWithoutGenericArguments}+";
                }
                else if (string.IsNullOrEmpty(Namespace))
                {
                    parent = string.Empty;
                }
                else
                {
                    parent = Namespace + ".";
                }
                // var parent = IsGenericParameter && DeclaringType != null || IsNested ? $"{DeclaringType.FullNameWithoutGenericArguments}+" : (string.IsNullOrEmpty(Namespace) ? string.Empty : Namespace + ".");
                var genericArgumentsText = HasGenericTypeArguments ? "[" + string.Join(",", GenericTypeArguments.Select(genericTypeArgument => $"[{genericTypeArgument.FullNameWithoutAssemblies}]")) + "]" : string.Empty;
                var name = UndecoratedName + genericArgumentsText + _nameModifiers.Value;
                var fullName = parent + name;

                return fullName;
            });

            _fullNameWithoutGenericArguments = new Lazy<string>(() =>
            {
                var parent = IsGenericParameter && DeclaringType != null || IsNested ? $"{DeclaringType.FullNameWithoutAssemblies}+" : (string.IsNullOrEmpty(Namespace) ? string.Empty : Namespace + ".");
                var name = UndecoratedName + _nameModifiers.Value;
                var fullName = parent + name;

                return fullName;
            });

            _name = new Lazy<string>(() => UndecoratedName + _nameModifiers.Value);
        }
    }
}
