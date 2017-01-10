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
        public delegate void BasicDelegate(string value);

        public enum ByteEnum : byte
        {
            A = byte.MinValue,
            B,
            C = byte.MaxValue
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

        public MoreSamples()
        {
            BasicEvent += BasicEventHandler1;
            BasicEvent += BasicEventHandler2;
            EventWithDeclaredAccessors += EventHandler;
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

        public ByteEnum ByteEnumValue { get; protected internal set; }

        public event BasicDelegate BasicEvent;

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

        public event EventHandler EventWithoutDeclaredAccessors;

        public static void Main(string[] args)
        {
        }

        public void BasicEventHandler1(string value)
        {
        }

        public void BasicEventHandler2(string value)
        {
        }

        public void DoSomething(ref string a)
        {
        }

        public void EventHandler(object sender, EventArgs e)
        {
        }

        protected virtual void OnEventWithoutDeclaredAccessors()
        {
            EventWithoutDeclaredAccessors?.Invoke(this, EventArgs.Empty);
        }
    }
}
