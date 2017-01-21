using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public abstract class MethodBase<TMethodBase, THandle, TToken> : MethodBase, ICodeElementWithTypedHandle<THandle, TToken> where TMethodBase : MethodBase<TMethodBase, THandle, TToken>
    {
        internal MethodBase(THandle handle, MetadataState metadataState) : base(new CodeElementKey<TMethodBase>(handle), metadataState)
        {
            MetadataHandle = handle;
            RawMetadata = (TToken) MetadataState.GetTokenForHandle(handle);
        }

        public override MemberTypes MemberType => this is IConstructor ? MemberTypes.Constructor : MemberTypes.Method;

        public TToken RawMetadata { get; }

        public THandle MetadataHandle { get; }
    }

    public abstract class MethodBase : MemberBase, IMethodBase
    {
        internal MethodBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        public abstract ImmutableArray<GenericParameter> GenericTypeParameters { get; }

        public bool IsConstructor => this is IConstructor && ".ctor".Equals(Name);

        public abstract bool IsGenericMethod { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetParameters" />
        public abstract ImmutableArray<IParameter> Parameters { get; }
    }
}
