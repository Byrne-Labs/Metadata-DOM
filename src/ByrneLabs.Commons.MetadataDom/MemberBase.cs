using System.Reflection.Metadata;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public abstract class MemberBase<TMemberBase, THandle, TToken> : RuntimeCodeElement, IMember, ICodeElementWithHandle<THandle, TToken> where TMemberBase : MemberBase<TMemberBase, THandle, TToken>
    {
        internal MemberBase(THandle handle, MetadataState metadataState) : base(new CodeElementKey<TMemberBase>(handle), metadataState)
        {
            MetadataHandle = handle;
            DowncastMetadataHandle = MetadataState.DowncastHandle(MetadataHandle).Value;
            MetadataToken = (TToken) MetadataState.GetTokenForHandle(MetadataHandle);
        }

        public abstract IEnumerable<CustomAttribute> CustomAttributes { get; }

        public bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public Handle DowncastMetadataHandle { get; }

        public THandle MetadataHandle { get; }

        public TToken MetadataToken { get; }

        public abstract TypeBase DeclaringType { get; }

        public abstract string FullName { get; }

        public abstract MemberTypes MemberType { get; }

        public abstract string Name { get; }

        public abstract string TextSignature { get; }
    }
}
