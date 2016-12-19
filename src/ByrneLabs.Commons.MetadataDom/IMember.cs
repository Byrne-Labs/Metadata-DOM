using System.Collections.Generic;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    public interface IMember
    {
        TypeBase DeclaringType { get; }

        string FullName { get; }

        MemberTypes MemberType { get; }

        string Name { get; }

        string TextSignature { get; }
    }
}
