using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    internal interface InterfaceSample
    {
        object LotsOfAttributes
        {
#if NETSTANDARD_1_1 || !DOTNET_STANDARD
            [DispId(1)]
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
#endif
            get;
        }

#if NETSTANDARD_1_1 || !DOTNET_STANDARD
        [DispId(1010)]
#endif
        int PropertyWithDispatchId { get; set; }

        int PropertyWithMarshal
        {
#if NETSTANDARD_1_1 || !DOTNET_STANDARD
            [return: MarshalAs(UnmanagedType.U4)]
#endif
            get;
            set;
        }

#if NETSTANDARD_1_1 || !DOTNET_STANDARD
        [DispId(1010)]
#endif
        int PropertyWithMarshalAndDispatchId
        {
#if NETSTANDARD_1_1 || !DOTNET_STANDARD
            [return: MarshalAs(UnmanagedType.U4)]
#endif
            get;
            set;
        }
#if CSHARP_V4
        object this[string Guid = "{00000000-0000-0000-0000-000000000000}", int ID = 0] { get; }
        int this[int something, int ID = 0]
        {
#if NETSTANDARD_1_1 || !DOTNET_STANDARD
            [return: MarshalAs(UnmanagedType.U4)]
#endif
            get;
        }
#endif
    }
}
