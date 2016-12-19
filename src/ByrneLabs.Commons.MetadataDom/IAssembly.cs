using System.Collections.Generic;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    public interface IAssembly
    {
        IEnumerable<CustomAttribute> CustomAttributes { get; }

        AssemblyFlags Flags { get; }

        AssemblyName Name { get; }
    }
}
