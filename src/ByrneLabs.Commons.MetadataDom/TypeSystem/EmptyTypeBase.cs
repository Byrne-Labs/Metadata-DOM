﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using TypeInfoToExpose = System.Reflection.TypeInfo;
using ConstructorInfoToExpose = System.Reflection.ConstructorInfo;
using MethodBaseToExpose = System.Reflection.MethodBase;
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using MethodInfoToExpose = System.Reflection.MethodInfo;
using PropertyInfoToExpose = System.Reflection.PropertyInfo;
using ModuleToExpose = System.Reflection.Module;
using AssemblyToExpose = System.Reflection.Assembly;
using EventInfoToExpose = System.Reflection.EventInfo;
using FieldInfoToExpose = System.Reflection.FieldInfo;
using MemberInfoToExpose = System.Reflection.MemberInfo;
using BaseTypeToExpose = System.Reflection.TypeInfo;

#else
using TypeInfoToExpose = ByrneLabs.Commons.MetadataDom.TypeInfo;
using ConstructorInfoToExpose = ByrneLabs.Commons.MetadataDom.ConstructorInfo;
using MethodBaseToExpose = ByrneLabs.Commons.MetadataDom.MethodBase;
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using MethodInfoToExpose = ByrneLabs.Commons.MetadataDom.MethodInfo;
using PropertyInfoToExpose = ByrneLabs.Commons.MetadataDom.PropertyInfo;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;
using AssemblyToExpose = ByrneLabs.Commons.MetadataDom.Assembly;
using EventInfoToExpose = ByrneLabs.Commons.MetadataDom.EventInfo;
using FieldInfoToExpose = ByrneLabs.Commons.MetadataDom.FieldInfo;
using MemberInfoToExpose = ByrneLabs.Commons.MetadataDom.MemberInfo;
using BaseTypeToExpose = ByrneLabs.Commons.MetadataDom.Type;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public abstract class EmptyTypeBase<TTypeBase, THandle, TToken> : EmptyTypeBase where TTypeBase : EmptyTypeBase<TTypeBase, THandle, TToken>
    {
        internal EmptyTypeBase(EmptyTypeBase<TTypeBase, THandle, TToken> unmodifiedType, TypeElementModifier typeElementModifier, MetadataState metadataState) : base(unmodifiedType, typeElementModifier, metadataState, new CodeElementKey<TTypeBase>(unmodifiedType, typeElementModifier))
        {
            MetadataHandle = unmodifiedType.MetadataHandle;
            RawMetadata = (TToken) MetadataState.GetRawMetadataForHandle(unmodifiedType.DowncastMetadataHandle);
        }

        internal EmptyTypeBase(EmptyTypeBase<TTypeBase, THandle, TToken> genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState) : base(genericTypeDefinition, genericTypeArguments, metadataState, new CodeElementKey<TTypeBase>(genericTypeDefinition, genericTypeDefinition, genericTypeArguments))
        {
            MetadataHandle = genericTypeDefinition.MetadataHandle;
            RawMetadata = (TToken) MetadataState.GetRawMetadataForHandle(genericTypeDefinition.DowncastMetadataHandle);
        }

        internal EmptyTypeBase(THandle handle, MetadataState metadataState) : base(new CodeElementKey<TTypeBase>(handle), metadataState)
        {
            MetadataHandle = handle;
            RawMetadata = (TToken) MetadataState.GetRawMetadataForHandle(handle);
        }

        internal EmptyTypeBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
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

    public abstract partial class EmptyTypeBase : TypeBase
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This is only valid for EmptyTypeBase")]
        internal EmptyTypeBase(EmptyTypeBase unmodifiedType, TypeElementModifier typeElementModifier, MetadataState metadataState, CodeElementKey key) : base(unmodifiedType, typeElementModifier, metadataState, key)
        {
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "This is only valid for EmptyTypeBase")]
        internal EmptyTypeBase(EmptyTypeBase genericTypeDefinition, IEnumerable<TypeBase> genericTypeArguments, MetadataState metadataState, CodeElementKey key) : base(genericTypeDefinition, genericTypeArguments, metadataState, key)
        {
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Invoked using reflection")]
        internal EmptyTypeBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        public override AssemblyToExpose Assembly => null;

        public override string AssemblyQualifiedName => null;

        public override TypeToExpose BaseType => null;

        public override bool ContainsGenericParameters => false;

        public override IEnumerable<MemberInfoToExpose> DeclaredMembers => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override MethodBaseToExpose DeclaringMethod => null;

        public override TypeToExpose DeclaringType => null;

        public override IEnumerable<Document> Documents => Enumerable.Empty<Document>();

        public override TypeToExpose[] GenericTypeParameters => Array.Empty<TypeToExpose>();

        public override IEnumerable<Type> ImplementedInterfaces => Enumerable.Empty<Type>();

        public override bool IsConstructedGenericType => false;

        public override bool IsDelegate => false;

        public override bool IsEnum => false;

        public override bool IsGenericParameter => false;

        public override bool IsGenericTypeDefinition => false;

        public override IEnumerable<Language> Languages => ImmutableArray<Language>.Empty;

        public override MemberTypes MemberType => MemberTypes.TypeInfo;

        public override ModuleToExpose Module => null;

        public override string Namespace => null;

        public override TypeToExpose ReflectedType => null;

        public override StructLayoutAttribute StructLayoutAttribute => null;

        public override RuntimeTypeHandle TypeHandle => throw NotSupportedHelper.NotValidForMetadata();

        public override ConstructorInfoToExpose TypeInitializer => null;

        internal override string MetadataNamespace => null;

        internal override string UndecoratedName => null;

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => Enumerable.Empty<CustomAttributeDataToExpose>().ToImmutableList();

        public override InterfaceMapping GetInterfaceMap(Type interfaceType) => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        protected override TypeAttributes GetAttributeFlagsImpl() => throw NotSupportedHelper.NotValidForMetadataType(GetType());
    }
#if NETSTANDARD2_0 || NET_FRAMEWORK

    public abstract partial class EmptyTypeBase
    {
    }
#else
    public abstract partial class EmptyTypeBase
    {
    }
#endif
}
