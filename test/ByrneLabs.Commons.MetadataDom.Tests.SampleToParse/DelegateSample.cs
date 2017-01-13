using System.Runtime.InteropServices;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    public delegate EnumSample EnumDelegate([In] int int1, [In] int int2, [MarshalAs(UnmanagedType.BStr)] [In] string str);
}
