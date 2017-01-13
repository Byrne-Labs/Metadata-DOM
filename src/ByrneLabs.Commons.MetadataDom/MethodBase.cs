using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public abstract class MethodBase<TMethodBase, THandle, TToken> : MethodBase, ICodeElementWithHandle<THandle, TToken> where TMethodBase : MethodBase<TMethodBase, THandle, TToken>
    {
        internal MethodBase(THandle handle, MetadataState metadataState) : base(new CodeElementKey<TMethodBase>(handle), metadataState)
        {
            DowncastMetadataHandle = MetadataState.DowncastHandle(MetadataHandle).Value;
            MetadataHandle = handle;
            MetadataToken = (TToken) MetadataState.GetTokenForHandle(MetadataHandle);
        }

        public override MemberTypes MemberType => this is IConstructor ? MemberTypes.Constructor : MemberTypes.Method;

        public Handle DowncastMetadataHandle { get; }

        public THandle MetadataHandle { get; }

        public TToken MetadataToken { get; }
    }

    public abstract class MethodBase : RuntimeCodeElement, IMethodBase
    {
        internal MethodBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        public abstract IEnumerable<CustomAttribute> CustomAttributes { get; }

        public bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public abstract TypeBase DeclaringType { get; }

        public abstract string FullName { get; }

        public abstract MemberTypes MemberType { get; }

        public abstract string Name { get; }

        public abstract string TextSignature { get; }

        public abstract IEnumerable<TypeBase> GenericArguments { get; }

        public bool IsConstructor => this is IConstructor;

        public bool IsGenericMethod => GenericArguments.Any();

        /// <inheritdoc cref="System.Reflection.Metadata.MethodDefinition.GetParameters" />
        public abstract IEnumerable<IParameter> Parameters { get; }
    }
}
