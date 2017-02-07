using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class TypeBase<TTypeBase, THandle, TToken> : TypeBase<TTypeBase, THandle>, ICodeElementWithTypedHandle<THandle, TToken> where TTypeBase : TypeBase
    {
        internal TypeBase(TypeBase<TTypeBase, THandle> baseType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(baseType, typeElementModifier, metadataState)
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
            if (key.Handle != null)
            {
                RawMetadata = (TToken)MetadataState.GetRawMetadataForHandle(key.Handle.Value);
            }
        }

        public TToken RawMetadata { get; }

        public virtual THandle MetadataHandle => KeyValue;
    }

    public abstract class TypeBase<TTypeBase, TKey> : TypeBase where TTypeBase : TypeBase
    {
        internal TypeBase(TypeBase<TTypeBase, TKey> baseType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(baseType, typeElementModifier, metadataState, new CodeElementKey<TTypeBase>(baseType, typeElementModifier))
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
        private readonly TypeElementModifier? _typeElementModifier;

        internal TypeBase(TypeBase baseType, TypeElementModifier typeElementModifier, MetadataState metadataState, CodeElementKey key) : this(key, metadataState)
        {
            BaseType = baseType;
            _typeElementModifier = typeElementModifier;
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

        protected abstract string MetadataNamespace { get; }

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

        public bool IsBoxed => BaseType?.IsBoxed == true || IsThisBoxed;

        public bool IsByRef => BaseType?.IsByRef == true || IsThisByRef;

        public bool IsByValue => BaseType?.IsByValue == true || IsThisByValue;

        public bool IsConstant => BaseType?.IsConstant == true || IsThisConstant;

        public virtual bool IsGenericType => BaseType?.IsGenericType == true || IsThisGenericType;

        public bool IsNested => DeclaringType != null;

        public bool IsPointer => BaseType?.IsPointer == true || IsThisPointer;

        public virtual bool IsValueType => BaseType?.IsValueType == true || IsThisValueType;

        public bool IsVolatile => BaseType?.IsVolatile == true || IsThisVolatile;

        public override string Name => _name.Value;

        public override string TextSignature => FullName;

        protected int ArrayDimensionCount => (IsThisArray ? 1 : 0) + (BaseType?.ArrayDimensionCount).GetValueOrDefault();

        protected bool IsThisArray => _typeElementModifier == TypeElementModifier.Array;

        protected bool IsThisBoxed => _typeElementModifier == TypeElementModifier.Boxed;

        protected bool IsThisByRef => _typeElementModifier == TypeElementModifier.ByRef;

        protected bool IsThisByValue => _typeElementModifier == TypeElementModifier.ByValue;

        protected bool IsThisConstant => _typeElementModifier == TypeElementModifier.Constant;

        protected bool IsThisGenericType { get; }

        protected bool IsThisPointer => _typeElementModifier == TypeElementModifier.Pointer;

        protected bool IsThisValueType => _typeElementModifier == TypeElementModifier.ValueType;

        protected bool IsThisVolatile => _typeElementModifier == TypeElementModifier.Volatile;

        protected int PointerCount => (IsThisPointer ? 1 : 0) + (BaseType?.PointerCount).GetValueOrDefault();

        internal virtual TypeBase BaseType { get; }

        internal virtual string FullNameWithoutGenericArguments => _fullNameWithoutGenericArguments.Value;

        private string CreateFullName(Func<TypeBase, string> nameGetter, bool includeGenericArguments)
        {
            string fullName;
            if (IsGenericParameter && DeclaringType == null)
            {
                fullName = Name;
            }
            else
            {
                var typeToUse = _typeElementModifier != null ? BaseType : this;
                string parent;
                if (typeToUse.IsNested && typeToUse.DeclaringType.IsGenericType)
                {
                    parent = (string.IsNullOrEmpty(typeToUse.DeclaringType.Namespace) ? string.Empty : typeToUse.DeclaringType.Namespace + ".") + typeToUse.DeclaringType.UndecoratedName + "+";
                }
                else if (typeToUse.IsGenericParameter && typeToUse.DeclaringType != null || typeToUse.IsNested)
                {
                    parent = $"{nameGetter(typeToUse.DeclaringType)}+";
                }
                else
                {
                    parent = string.Empty;
                }
                string @namespace;
                if (typeToUse.IsNested && !typeToUse.IsGenericParameter && typeToUse.MetadataNamespace?.StartsWith("<") == true && typeToUse.Name.Contains(">"))
                {
                    @namespace = typeToUse.MetadataNamespace + ".";
                }
                else if (string.IsNullOrEmpty(typeToUse.Namespace) || typeToUse.IsNested && !typeToUse.IsGenericParameter)
                {
                    @namespace = string.Empty;
                }
                else
                {
                    @namespace = typeToUse.Namespace + ".";
                }
                string genericArgumentsText;
                if (includeGenericArguments && typeToUse.HasGenericTypeArguments)
                {
                    genericArgumentsText = "[" + string.Join(",", typeToUse.GenericTypeArguments.Select(genericTypeArgument => genericTypeArgument.FullNameWithoutAssemblies ?? genericTypeArgument.Name)) + "]";
                }

                else
                {
                    genericArgumentsText = string.Empty;
                }

                if (typeToUse == this)
                {
                    fullName = parent + @namespace + typeToUse.UndecoratedName + genericArgumentsText + _nameModifiers.Value;
                }
                else
                {
                    fullName = parent + @namespace + typeToUse.UndecoratedName  + _nameModifiers.Value + genericArgumentsText;
                }
            }
            return fullName;
        }

        private void Initialize()
        {
            _nameModifiers = new Lazy<string>(() => string.Concat(Enumerable.Repeat("[]", ArrayDimensionCount)) + new string('*', PointerCount) + (IsByRef ? "&" : string.Empty));
            _fullName = new Lazy<string>(() => CreateFullName(type => type.FullName, true));
            _fullNameWithoutAssemblies = new Lazy<string>(() => CreateFullName(type => type.FullNameWithoutAssemblies, true));
            _fullNameWithoutGenericArguments = new Lazy<string>(() => CreateFullName(type => type.FullNameWithoutGenericArguments, false));
            _name = new Lazy<string>(() => UndecoratedName + _nameModifiers.Value);
        }
    }
}
