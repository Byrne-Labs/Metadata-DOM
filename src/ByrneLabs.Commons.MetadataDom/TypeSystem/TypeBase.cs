using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using TypeInfoToExpose = System.Reflection.TypeInfo;
using TypeToExpose = System.Type;
using EventInfoToExpose = System.Reflection.EventInfo;
using FieldInfoToExpose = System.Reflection.FieldInfo;
using MemberInfoToExpose = System.Reflection.MemberInfo;
using ConstructorInfoToExpose = System.Reflection.ConstructorInfo;
using MethodInfoToExpose = System.Reflection.MethodInfo;
using PropertyInfoToExpose = System.Reflection.PropertyInfo;

#else
using TypeInfoToExpose = ByrneLabs.Commons.MetadataDom.TypeInfo;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using EventInfoToExpose = ByrneLabs.Commons.MetadataDom.EventInfo;
using FieldInfoToExpose = ByrneLabs.Commons.MetadataDom.FieldInfo;
using MemberInfoToExpose = ByrneLabs.Commons.MetadataDom.MemberInfo;
using ConstructorInfoToExpose = ByrneLabs.Commons.MetadataDom.ConstructorInfo;
using MethodInfoToExpose = ByrneLabs.Commons.MetadataDom.MethodInfo;
using PropertyInfoToExpose = ByrneLabs.Commons.MetadataDom.PropertyInfo;

#endif

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

        public override int MetadataToken => MetadataHandle.GetHashCode();

        public TToken RawMetadata { get; }
    }

    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {FullNameWithoutAssemblies}")]
    [PublicAPI]
    public abstract class TypeBase : TypeInfo, IManagedCodeElement
    {
        internal TypeBase UnmodifiedType { get; }
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
            GenericTypeArguments = Array.Empty<TypeToExpose>();
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
            IsThisGenericType = true;
            GenericTypeDefinition = genericTypeDefinition;
            GenericTypeArguments = genericTypeArguments.Cast<TypeToExpose>().ToArray();
            Initialize();
        }

        internal TypeBase(CodeElementKey key, MetadataState metadataState)
        {
            Key = key;
            GenericTypeArguments = Array.Empty<TypeToExpose>();
            MetadataState = metadataState;
            Initialize();
        }

        public override int ArrayRank { get; }

        public override string AssemblyQualifiedName => Assembly == null ? FullName : $"{FullName}, {Assembly.FullName}";

        public Handle? DowncastMetadataHandle => ((IManagedCodeElement) this).Key.Handle;

        public override TypeInfoToExpose ElementType { get; }

        public override string FullName => _fullName.Value;

        public override TypeToExpose[] GenericTypeArguments { get; }

        public override sealed TypeInfoToExpose GenericTypeDefinition { get; }

        public override sealed bool HasGenericTypeArguments => GenericTypeArguments.Any();

        public override sealed bool IsBoxed => UnmodifiedType?.IsBoxed == true || IsThisBoxed;

        public override sealed bool IsByValue => UnmodifiedType?.IsByValue == true || IsThisByValue;

        public override sealed bool IsConstant => UnmodifiedType?.IsConstant == true || IsThisConstant;

        public override bool IsGenericType => UnmodifiedType?.IsGenericType == true || IsThisGenericType;

        public override sealed bool IsVolatile => UnmodifiedType?.IsVolatile == true || IsThisVolatile;

        public override int MetadataToken => DowncastMetadataHandle.HasValue ? DowncastMetadataHandle.Value.GetHashCode() : 0;

        public override string Name => _name.Value;

        public override string TextSignature => FullNameWithoutAssemblies;

        protected bool IsThisArray => TypeElementModifier == TypeSystem.TypeElementModifier.Array;

        protected bool IsThisBoxed => TypeElementModifier == TypeSystem.TypeElementModifier.Boxed;

        protected bool IsThisByRef => TypeElementModifier == TypeSystem.TypeElementModifier.ByRef;

        protected bool IsThisByValue => TypeElementModifier == TypeSystem.TypeElementModifier.ByValue;

        protected bool IsThisConstant => TypeElementModifier == TypeSystem.TypeElementModifier.Constant;

        protected bool IsThisGenericType { get; }

        protected bool IsThisPointer => TypeElementModifier == TypeSystem.TypeElementModifier.Pointer;

        protected bool IsThisValueType => TypeElementModifier == TypeSystem.TypeElementModifier.ValueType;

        protected bool IsThisVolatile => TypeElementModifier == TypeSystem.TypeElementModifier.Volatile;

        internal abstract string MetadataNamespace { get; }

        internal int ArrayDimensionCount => (IsThisArray ? 1 : 0) + (UnmodifiedType?.ArrayDimensionCount).GetValueOrDefault();

        internal string FullNameWithoutAssemblies => _fullNameWithoutAssemblies.Value;

        internal string FullNameWithoutGenericArguments => _fullNameWithoutGenericArguments.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        internal int PointerCount => (IsThisPointer ? 1 : 0) + (UnmodifiedType?.PointerCount).GetValueOrDefault();

        internal TypeElementModifier? TypeElementModifier { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override int GetArrayRank() => ArrayRank;

        public override sealed TypeToExpose GetElementType() => ElementType;

        public override EventInfoToExpose[] GetEvents(BindingFlags bindingAttr) => throw NotSupportedHelper.FutureVersion();

        public override FieldInfoToExpose[] GetFields(BindingFlags bindingAttr) => throw NotSupportedHelper.FutureVersion();

        public override TypeToExpose[] GetGenericArguments() => throw new NotSupportedException();

        public override sealed TypeToExpose GetGenericTypeDefinition() => GenericTypeDefinition;

        public override MemberInfoToExpose[] GetMembers(BindingFlags bindingAttr) => throw NotSupportedHelper.FutureVersion();

        protected override ConstructorInfoToExpose GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => throw NotSupportedHelper.FutureVersion();

        protected override MethodInfoToExpose GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => throw NotSupportedHelper.FutureVersion();

        protected override PropertyInfoToExpose GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) => throw NotSupportedHelper.FutureVersion();

        protected override sealed bool HasElementTypeImpl() => ElementType != null;

        protected override sealed bool IsArrayImpl() => UnmodifiedType?.IsArray == true || IsThisArray;

        protected override sealed bool IsByRefImpl() => UnmodifiedType?.IsByRef == true || IsThisByRef;

        protected override sealed bool IsPointerImpl() => UnmodifiedType?.IsPointer == true || IsThisPointer;

        protected override bool IsPrimitiveImpl() => false;

        protected override sealed bool IsValueTypeImpl() => UnmodifiedType?.IsValueType == true || IsThisValueType;

        private void Initialize()
        {
            _fullName = new Lazy<string>(this.GetFullName);
            _fullNameWithoutAssemblies = new Lazy<string>(this.GetFullNameWithoutAssemblies);
            _fullNameWithoutGenericArguments = new Lazy<string>(this.GetFullNameWithoutGenericArguments);
            _name = new Lazy<string>(() => UndecoratedName + this.GetNameModifiers());
        }
    }
}
