using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    public interface IAssembly
    {
        ImmutableArray<CustomAttribute> CustomAttributes { get; }

        AssemblyFlags Flags { get; }

        AssemblyName Name { get; }
    }
}
