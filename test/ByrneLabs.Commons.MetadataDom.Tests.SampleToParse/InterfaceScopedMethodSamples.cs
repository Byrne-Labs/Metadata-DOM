using System;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    public interface Interface1
    {
        object ReadOnlyValue { get; }

        object ReadWriteValue { get; set; }

        object WriteOnlyValue { set; }

        void DoSomething();
    }

    public interface Interface2
    {
        object ReadOnlyValue { get; }

        object ReadWriteValue { get; set; }

        object WriteOnlyValue { set; }

        void DoSomething();
    }

    public interface Interface3
    {
        object ReadOnlyValue { get; }

        object ReadWriteValue { get; set; }

        object WriteOnlyValue { set; }

        void DoSomething();
    }

    public class ImplementingClass : Interface1, Interface2, Interface3
    {
        void Interface1.DoSomething()
        {
        }

        public object ReadOnlyValue => null;

        public object ReadWriteValue
        {
            get => null;
            set
            {
                var a = value;
            }
        }

        public object WriteOnlyValue
        {
            set
            {
                var a = value;
            }
        }

        void Interface2.DoSomething()
        {
        }

        object Interface2.ReadOnlyValue => null;

        object Interface2.ReadWriteValue
        {
            get => null;
            set
            {
                var a = value;
            }
        }

        object Interface2.WriteOnlyValue
        {
            set
            {
                var a = value;
            }
        }

        void Interface3.DoSomething()
        {
            throw new NotSupportedException("This will be supported in the future");
        }

        object Interface3.ReadOnlyValue => null;

        object Interface3.ReadWriteValue
        {
            get => null;
            set
            {
                var a = value;
            }
        }

        object Interface3.WriteOnlyValue
        {
            set
            {
                var a = value;
            }
        }

        public void DoSomething()
        {
        }
    }
}
