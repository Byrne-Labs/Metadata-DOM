﻿using System.Runtime.InteropServices;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
#if NETSTANDARD_1_1 || !DOTNET_STANDARD
    public delegate EnumSample EnumDelegate([In] int int1, [In] int int2, [MarshalAs(UnmanagedType.BStr)] [In] string str);
#else
    public delegate EnumSample EnumDelegate(int int1, int int2, string str);
#endif
}
