using System.Collections.Immutable;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public interface IAssembly
    {
        ImmutableArray<CustomAttribute> CustomAttributes { get; }

        AssemblyFlags Flags { get; }

        AssemblyName Name { get; }
    }
}
