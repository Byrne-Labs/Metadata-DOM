using System;
using System.Collections.Generic;

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

        public event EventHandler EventWithoutDeclaredAccessors;

        public event EventHandler EventWithDeclaredAccessors
        {
            add
            {
                throw new NotImplementedException();
            }
            remove
            {
                throw new NotImplementedException();
            }
        }

        public string this[int index, string index2]
        {
            get
            {
                return "a";
            }
            set
            {
                var a = "a";
                DoSomething(ref a);
            }
        }

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

        protected virtual void OnEventWithoutDeclaredAccessors()
        {
            EventWithoutDeclaredAccessors?.Invoke(this, EventArgs.Empty);
        }
    }
}
