using System;
using System.Collections.Generic;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    public interface IModule
    {
        IAssembly Assembly { get; }

        string Name { get; }
    }
}
