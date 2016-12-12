using System;
using System.Collections.Generic;
using System.Text;

namespace ByrneLabs.Commons.MetadataDom
{
    [Flags]
    internal enum TypeElementModifiers
    {
        None = 0,
        Array = 1,
        ByRef = 2,
        Generic = 4,
        Pointer = 8
    }
}
