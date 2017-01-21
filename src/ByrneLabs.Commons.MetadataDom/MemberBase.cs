using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public abstract class MemberBase : RuntimeCodeElement, IMember
    {
        internal MemberBase(Handle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
        }

        internal MemberBase(CodeElementKey key, MetadataState metadataState) : base(key, metadataState)
        {
        }

        public bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public ModuleDefinition Module => MetadataState.ModuleDefinition;

        public abstract ImmutableArray<CustomAttribute> CustomAttributes { get; }

        public abstract TypeBase DeclaringType { get; }

        public abstract string FullName { get; }

        public abstract MemberTypes MemberType { get; }

        public abstract string Name { get; }

        public abstract string TextSignature { get; }

        public override string ToString() => TextSignature;
    }
}
