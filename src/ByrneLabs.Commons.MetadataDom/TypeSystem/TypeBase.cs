using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public abstract class TypeBase<TTypeBase, THandle, TToken> : TypeBase where TTypeBase : TypeBase
    {
        internal TypeBase(TypeBase<TTypeBase, THandle, TToken> unmodifiedType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(unmodifiedType, typeElementModifier, metadataState, new CodeElementKey<TTypeBase>(unmodifiedType, typeElementModifier))
        {
            MetadataHandle = unmodifiedType.MetadataHandle;
            RawMetadata = (TToken) MetadataState.GetRawMetadataForHandle(unmodifiedType.DowncastMetadataHandle);
        }

        internal TypeBase(TypeBase<TTypeBase, THandle, TToken> genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState, new CodeElementKey<TTypeBase>(genericTypeDefinition, genericTypeDefinition, genericTypeArguments))
        {
            MetadataHandle = genericTypeDefinition.MetadataHandle;
            RawMetadata = (TToken) MetadataState.GetRawMetadataForHandle(genericTypeDefinition.DowncastMetadataHandle);
        }

        internal TypeBase(THandle handle, MetadataState metadataState) : base(new CodeElementKey<TTypeBase>(handle), metadataState)
        {
            MetadataHandle = handle;
            RawMetadata = (TToken) MetadataState.GetRawMetadataForHandle(handle);
        }

        internal TypeBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
            if (key.Handle != null)
            {
                RawMetadata = (TToken) MetadataState.GetRawMetadataForHandle(key.Handle.Value);
            }
        }

        public THandle MetadataHandle { get; }

        public override int MetadataToken => Key.Handle.Value.GetHashCode();

        public TToken RawMetadata { get; }
    }

    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {FullNameWithoutAssemblies}")]
    [PublicAPI]
    public abstract class TypeBase : TypeInfo, IManagedCodeElement
    {
        private readonly Lazy<Type[]> _genericTypeArguments;
        private Lazy<string> _fullName;
        private Lazy<string> _fullNameWithoutAssemblies;
        private Lazy<string> _fullNameWithoutGenericArguments;
        private Lazy<string> _name;

        internal TypeBase(TypeBase unmodifiedType, TypeElementModifier typeElementModifier, MetadataState metadataState, CodeElementKey key)
        {
            Key = key;
            UnmodifiedType = unmodifiedType;
            TypeElementModifier = typeElementModifier;
            MetadataState = metadataState;
            if (IsArrayImpl() || IsPointerImpl())
            {
                ElementType = unmodifiedType;
            }
            if (IsArrayImpl())
            {
                ArrayRank = 1;
            }
            _genericTypeArguments = new Lazy<Type[]>(Array.Empty<Type>);
            Initialize();
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This should not be used for other implementations of TypeInfo")]
        internal TypeBase(TypeBase genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState, CodeElementKey key)
        {
            Key = key;
            if (genericTypeArguments.Any(x => x == null))
            {
                throw new ArgumentException();
            }

            MetadataState = metadataState;
            GenericTypeDefinition = genericTypeDefinition;
            _genericTypeArguments = new Lazy<Type[]>(() => DeclaringType?.GenericTypeArguments == null ? genericTypeArguments.ToArray<Type>() : genericTypeArguments.Union(DeclaringType.GenericTypeArguments).ToArray());
            Initialize();
        }

        internal TypeBase(CodeElementKey key, MetadataState metadataState)
        {
            Key = key;
            _genericTypeArguments = new Lazy<Type[]>(Array.Empty<Type>);
            MetadataState = metadataState;
            Initialize();
        }

        public override int ArrayRank { get; }

        public override string AssemblyQualifiedName => Assembly == null ? FullName : $"{FullName}, {Assembly.FullName}";

        public Handle? DowncastMetadataHandle => ((IManagedCodeElement) this).Key.Handle;

        public override TypeInfo ElementType { get; }

        public override string FullName => _fullName.Value;

        public override string FullTextSignature => FullNameWithoutAssemblies;

        public sealed override TypeInfo GenericTypeDefinition { get; }

        public sealed override bool IsBoxed => UnmodifiedType?.IsBoxed == true || IsThisBoxed;

        public sealed override bool IsByValue => UnmodifiedType?.IsByValue == true || IsThisByValue;

        public sealed override bool IsConstant => UnmodifiedType?.IsConstant == true || IsThisConstant;

        public override bool IsGenericType => UnmodifiedType?.IsGenericType == true || IsThisGenericType;

        public sealed override bool IsVolatile => UnmodifiedType?.IsVolatile == true || IsThisVolatile;

        public override int MetadataToken => DowncastMetadataHandle.HasValue ? DowncastMetadataHandle.Value.GetHashCode() : 0;

        public override string Name => _name.Value;

        internal abstract string MetadataNamespace { get; }

        internal int ArrayDimensionCount => (IsThisArray ? 1 : 0) + (UnmodifiedType?.ArrayDimensionCount).GetValueOrDefault();

        internal string FullNameWithoutAssemblies => _fullNameWithoutAssemblies.Value;

        internal string FullNameWithoutGenericArguments => _fullNameWithoutGenericArguments.Value;

        internal Type[] GenericArgumentsWithoutParameters => _genericTypeArguments.Value;

        internal bool IsThisArray => TypeElementModifier == TypeSystem.TypeElementModifier.Array;

        internal bool IsThisBoxed => TypeElementModifier == TypeSystem.TypeElementModifier.Boxed;

        internal bool IsThisByRef => TypeElementModifier == TypeSystem.TypeElementModifier.ByRef;

        internal bool IsThisByValue => TypeElementModifier == TypeSystem.TypeElementModifier.ByValue;

        internal bool IsThisConstant => TypeElementModifier == TypeSystem.TypeElementModifier.Constant;

        internal bool IsThisGenericType => _genericTypeArguments.Value.Any();

        internal bool IsThisPointer => TypeElementModifier == TypeSystem.TypeElementModifier.Pointer;

        internal bool IsThisValueType => TypeElementModifier == TypeSystem.TypeElementModifier.ValueType;

        internal bool IsThisVolatile => TypeElementModifier == TypeSystem.TypeElementModifier.Volatile;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        internal int PointerCount => (IsThisPointer ? 1 : 0) + (UnmodifiedType?.PointerCount).GetValueOrDefault();

        internal TypeElementModifier? TypeElementModifier { get; }

        internal TypeBase UnmodifiedType { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override int GetArrayRank() => ArrayRank;

        public sealed override Type GetElementType() => ElementType;

        public override Type[] GetGenericArguments() => _genericTypeArguments.Value.Any() ? _genericTypeArguments.Value : GenericTypeParameters;

        public sealed override Type GetGenericTypeDefinition() => GenericTypeDefinition;

        public override int GetHashCode() => MetadataToken | 12345;

        protected sealed override bool HasElementTypeImpl() => ElementType != null;

        protected sealed override bool IsArrayImpl() => UnmodifiedType?.IsArray == true || IsThisArray;

        protected sealed override bool IsByRefImpl() => UnmodifiedType?.IsByRef == true || IsThisByRef;

        protected sealed override bool IsPointerImpl() => UnmodifiedType?.IsPointer == true || IsThisPointer;

        protected override bool IsPrimitiveImpl() => false;

        protected sealed override bool IsValueTypeImpl() => UnmodifiedType?.IsValueType == true || IsThisValueType || IsEnum;

        private void Initialize()
        {
            _fullName = new Lazy<string>(this.GetFullName);
            _fullNameWithoutAssemblies = new Lazy<string>(this.GetFullNameWithoutAssemblies);
            _fullNameWithoutGenericArguments = new Lazy<string>(this.GetFullNameWithoutGenericArguments);
            _name = new Lazy<string>(() => UndecoratedName + this.GetNameModifiers());
        }
    }
}
