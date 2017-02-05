using System;
using System.Collections.Generic;
using System.Text;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    public interface Interface1
    {
        void DoSomething();
    }

    public interface Interface2
    {
        void DoSomething();
    }

    public interface Interface3
    {
        void DoSomething();
    }

    public class ImplementingClass : Interface1, Interface2, Interface3
    {
        void Interface1.DoSomething()
        {
        }

        void Interface2.DoSomething()
        {
        }

        public void DoSomething()
        {
        }
    }
}
