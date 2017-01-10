using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    internal interface InterfaceSample
    {
        object this[string Guid = "{00000000-0000-0000-0000-000000000000}", int ID = 0] { get; }

        int this[int something, int ID = 0]
        {
            [return: MarshalAs(UnmanagedType.U4)]
            get;
        }

        object LotsOfAttributes
        {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            get;
        }

        [DispId(1010)]
        int PropertyWithDispatchId { get; set; }

        int PropertyWithMarshal
        {
            [return: MarshalAs(UnmanagedType.U4)]
            get;
            set;
        }

        [DispId(1010)]
        int PropertyWithMarshalAndDispatchId
        {
            [return: MarshalAs(UnmanagedType.U4)]
            get;
            set;
        }
    }
}
