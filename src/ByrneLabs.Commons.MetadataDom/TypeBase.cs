using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class TypeBase<TTypeBase, THandle, TToken> : TypeBase<TTypeBase, THandle>, ICodeElementWithHandle<THandle, TToken> where TTypeBase : TypeBase
    {
        internal TypeBase(TypeBase<TTypeBase, THandle> baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState) : base(baseType, typeElementModifiers, metadataState)
        {
            DowncastMetadataHandle = MetadataState.DowncastHandle(MetadataHandle).Value;
            MetadataToken = (TToken) MetadataState.GetTokenForHandle(MetadataHandle);
        }

        internal TypeBase(TypeBase<TTypeBase, THandle> genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState)
        {
            DowncastMetadataHandle = MetadataState.DowncastHandle(MetadataHandle).Value;
            MetadataToken = (TToken) MetadataState.GetTokenForHandle(MetadataHandle);
        }

        internal TypeBase(THandle handle, MetadataState metadataState) : base(handle, metadataState)
        {
            DowncastMetadataHandle = MetadataState.DowncastHandle(MetadataHandle).Value;
            MetadataToken = (TToken) MetadataState.GetTokenForHandle(MetadataHandle);
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

    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {FullName}")]
    //[PublicAPI]
    public abstract class TypeBase : RuntimeCodeElement, IMember
    {
        private Lazy<string> _fullName;

        internal TypeBase(TypeBase baseType, TypeElementModifiers typeElementModifiers, MetadataState metadataState, CodeElementKey key) : this(key, metadataState)
        {
            IsArray = typeElementModifiers.HasFlag(TypeElementModifiers.Array);
            IsByRef = typeElementModifiers.HasFlag(TypeElementModifiers.ByRef);
            IsGenericType = typeElementModifiers.HasFlag(TypeElementModifiers.GenericType);
            IsPointer = typeElementModifiers.HasFlag(TypeElementModifiers.Pointer);

            if (IsArray || IsPointer)
            {
                ElementType = baseType;
            }
            GenericTypeArguments = new TypeBase[] { };
            Initialize();
        }

        internal TypeBase(TypeBase genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState, CodeElementKey key) : this(key, metadataState)
        {
            IsGenericType = true;
            GenericTypeDefinition = genericTypeDefinition;
            GenericTypeArguments = genericTypeArguments.Select(typeArgument => (TypeBase) MetadataState.GetCodeElement(typeArgument.GetType(), typeArgument, TypeElementModifiers.GenericArgument)).ToArray();
            Initialize();
        }

        internal TypeBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
            GenericTypeArguments = new TypeBase[] { };
            Initialize();
        }

        public abstract IAssembly Assembly { get; }

        public abstract bool IsGenericParameter { get; }

        public abstract string Namespace { get; }

        public virtual string AssemblyQualifiedName => Assembly == null ? FullName : $"{FullName}, {Assembly.Name.FullName}";

        /// <summary>When overridden in a derived class, returns the <see cref="TypeBase" /> of the object encompassed or referred to by the current array, pointer or reference type.</summary>
        /// <returns>The <see cref="TypeBase" /> of the object encompassed or referred to by the current array, pointer, or reference type, or null if the current
        ///     <see cref="TypeBase" /> is not an array or a pointer, or is not passed by reference, or represents a generic type or a type parameter in the definition of a generic type or generic method.</returns>
        public TypeBase ElementType { get; }

        public IEnumerable<TypeBase> GenericTypeArguments { get; }

        public TypeBase GenericTypeDefinition { get; }

        public bool HasGenericTypeArguments => GenericTypeArguments.Any();

        public bool IsArray { get; }

        public bool IsByRef { get; }

        public bool IsGenericType { get; }

        public bool IsNested => DeclaringType != null;

        public bool IsPointer { get; }

        public abstract TypeBase DeclaringType { get; }

        public virtual string FullName => _fullName.Value;

        public abstract MemberTypes MemberType { get; }

        public abstract string Name { get; }

        public string TextSignature => FullName;

        private void Initialize()
        {
            _fullName = new Lazy<string>(() =>
            {
                var parent = IsNested ? DeclaringType.FullName + "+" : (string.IsNullOrEmpty(Namespace) ? string.Empty : Namespace + ".");
                var genericArgumentsText = HasGenericTypeArguments ? "[" + string.Join(", ", GenericTypeArguments.Select(genericTypeArgument => $"[{genericTypeArgument.AssemblyQualifiedName}]")) + "]" : string.Empty;

                return parent + Name + genericArgumentsText + (IsArray ? "[]" : string.Empty) + (IsByRef ? "&" : string.Empty) + (IsPointer ? "*" : string.Empty);
            });
        }
    }
}
