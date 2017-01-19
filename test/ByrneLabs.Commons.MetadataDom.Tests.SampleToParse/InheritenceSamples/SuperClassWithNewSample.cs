using System;
using System.Collections.Generic;
using System.Text;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.InheritenceSamples
{
    public class SuperClassWithNewSample : BaseClassSample
    {
        private int privateField;
        protected int protectedField;
        protected internal int protectedInternalField;
        internal int internalField;
        public int publicField;

        private int PrivateProperty { get; set; }
        protected new int ProtectedProperty { get; set; }
        protected internal new int ProtectedInternalProperty { get; set; }
        internal new int InternalProperty { get; set; }
        public new int PublicProperty { get; set; }

        protected new virtual int ProtectedVirtualProperty { get; set; }
        protected internal new virtual int ProtectedInternalVirtualProperty { get; set; }
        internal new virtual int InternalVirtualProperty { get; set; }
        public new virtual int PublicVirtualProperty { get; set; }

        private  int PrivateMethod() => 0;

        protected new int ProtectedMethod() => 0;

        protected internal new int ProtectedInternalMethod() => 0;

        internal new int InternalMethod() => 0;

        public new int PublicMethod() => 0;

        protected new virtual int ProtectedVirtualMethod() => 0;

        protected internal new virtual int ProtectedInternalVirtualMethod() => 0;

        internal new virtual int InternalVirtualMethod() => 0;

        public new virtual int PublicVirtualMethod() => 0;

    }
}
