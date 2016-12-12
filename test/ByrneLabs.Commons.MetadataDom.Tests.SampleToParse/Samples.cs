using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    public unsafe class Samples
    {
        public int IntValue = 1;
        public string[] StringArray = { "asdf", null, string.Empty };

        public GenericClass<Samples, Exception, int, Dictionary<string, string>, MoreSamples, List<MoreSamples>> GenericClassInstance { get; set; }

        public void DoStuff()
        {
            fixed (int* intPointer = &IntValue)
            {
                StringArray[1] = intPointer->ToString();
            }
        }
    }

    public class MoreSamples : Samples
    {
        public enum ByteEnum : byte
        {
            A,
            B,
            C
        }

        public enum NotAFlag
        {
            A,
            B,
            C
        }

        [Flags]
        public enum SomeFlags
        {
            A = 1,
            B = 2,
            C = 4,
            BC = 3
        }

        public static void Main(string[] args)
        {
        }

        public void DoSomething(ref string a)
        {
        }
    }
}
