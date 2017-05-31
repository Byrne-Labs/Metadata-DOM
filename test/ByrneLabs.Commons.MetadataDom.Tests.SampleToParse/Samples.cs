using System;
using System.Collections.Generic;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    public unsafe class Samples<T>
    {
        private const int constInt = 1;
        private readonly int readonlyInt = 1;
        public int IntValue = 1;
        public string[] StringArray = { "asdf", null, string.Empty };
        private Func<string> getValue;
        private volatile int volatileInt;

        public GenericClass<T[], object[], Samples<object[]>, object, int, Dictionary<string, string>, MoreSamples, List<MoreSamples>> GenericClassInstance { get; set; }

        public object Item
        {
            get => null;
            set
            {
                var a = value;
            }
        }

        public event EventHandler EventWithoutDeclaredAccessors;

        public void DoStuff()
        {
            fixed (int* intPointer = &IntValue)
            {
                StringArray[1] = intPointer->ToString();
            }
            GenericClassInstance = new GenericClass<T[], object[], Samples<object[]>, object, int, Dictionary<string, string>, MoreSamples, List<MoreSamples>>((int[]) null);
        }

        protected virtual void OnEventWithoutDeclaredAccessors()
        {
#if CSHARP_V6
            EventWithoutDeclaredAccessors?.Invoke(this, EventArgs.Empty);
#endif
        }
    }

    public sealed class MoreSamples : Samples<object[]>
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

        public ByteEnum ByteEnumValue
        {
            get
            {
                return default(ByteEnum);
            }
            protected internal set
            {
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
#if CSHARP_V3
                var a = "a";
#else
                string a = "a";
#endif
                DoSomething(ref a);
            }
        }

        public int SomeInt => 0;

#if CSHARP_V6
        public int SomeRedirectedInt => SomeInt;
#endif

        public event BasicDelegate BasicEvent;

        public event EventHandler EventWithDeclaredAccessors
        {
            add => throw new NotSupportedException("This will be supported in the future");
            remove => throw new NotSupportedException("This will be supported in the future");
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
    }
}
