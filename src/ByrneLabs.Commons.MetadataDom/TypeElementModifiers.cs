using System;

namespace ByrneLabs.Commons.MetadataDom
{
    [Flags]
    internal enum TypeElementModifiers
    {
        None = 0,
        Array = 1,
        ByRef = 2,
        GenericType = 4,
        GenericArgument = 8,
        Pointer = 16
    }
}
