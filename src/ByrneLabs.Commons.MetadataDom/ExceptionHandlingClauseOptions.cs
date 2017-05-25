#if !(NETSTANDARD2_0 || NET_FRAMEWORK)
using System;

namespace ByrneLabs.Commons.MetadataDom
{
    [Flags]
    public enum ExceptionHandlingClauseOptions
    {
        Clause = 0x0,
        Filter = 0x1,
        Finally = 0x2,
        Fault = 0x4
    }
}
#endif
