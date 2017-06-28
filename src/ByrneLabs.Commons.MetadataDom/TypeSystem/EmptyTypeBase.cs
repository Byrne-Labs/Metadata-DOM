using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

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

    public abstract class EmptyTypeBase : TypeBase
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

        public override System.Reflection.Assembly Assembly => null;

        public override string AssemblyQualifiedName => null;

        public override Type BaseType => null;

        public override bool ContainsGenericParameters => false;

        public override IEnumerable<MemberInfo> DeclaredMembers => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override MethodBase DeclaringMethod => null;

        public override Type DeclaringType => null;

        public override IEnumerable<Document> Documents => Enumerable.Empty<Document>();

        public override Type[] GenericTypeParameters => Array.Empty<Type>();

        public override IEnumerable<Type> ImplementedInterfaces => Enumerable.Empty<Type>();

        public override bool IsConstructedGenericType => false;

        public override bool IsDelegate => false;

        public override bool IsEnum => false;

        public override bool IsGenericParameter => false;

        public override bool IsGenericTypeDefinition => false;

        public override IEnumerable<Language> Languages => ImmutableArray<Language>.Empty;

        public override MemberTypes MemberType => MemberTypes.TypeInfo;

        public override System.Reflection.Module Module => null;

        public override string Namespace => null;

        public override Type ReflectedType => null;

        public override IEnumerable<MetadataDom.SequencePoint> SequencePoints => null;

        public override string SourceCode => null;

        public override StructLayoutAttribute StructLayoutAttribute => null;

        public override RuntimeTypeHandle TypeHandle => throw NotSupportedHelper.NotValidForMetadata();

        internal override string MetadataNamespace => null;

        internal override string UndecoratedName => null;

        public override IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() => Enumerable.Empty<System.Reflection.CustomAttributeData>().ToImmutableList();

        public override InterfaceMapping GetInterfaceMap(Type interfaceType) => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        protected override TypeAttributes GetAttributeFlagsImpl() => throw NotSupportedHelper.NotValidForMetadataType(GetType());
    }
}
