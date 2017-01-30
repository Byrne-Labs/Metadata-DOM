using System;

namespace ByrneLabs.Commons.MetadataDom
{
    [Flags]
    internal enum TypeElementModifiers
    {
        Array = 1,
        ByRef = 2,
        GenericType = 4,
        Volatile = 8,
        Pointer = 16,
        Constant = 32,
        ByValue = 64,
        Boxed = 128
    }
}
