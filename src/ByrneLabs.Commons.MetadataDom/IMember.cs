using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public interface IMember
    {
        TypeBase DeclaringType { get; }

        Handle DowncastMetadataHandle { get; }

        string FullName { get; }

        MemberTypes MemberType { get; }

        int MetadataToken { get; }

        string Name { get; }

        string TextSignature { get; }
    }
}
