using System;
using System.Collections.Generic;
using System.Text;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.InheritenceSamples
{
    public class SuperClassWithOverrideSample:BaseClassSample
    {
        private int privateField;
        protected int protectedField;
        protected internal int protectedInternalField;
        internal int internalField;
        public int publicField;

        private  int PrivateProperty { get; set; }
        protected int ProtectedProperty { get; set; }
        protected internal int ProtectedInternalProperty { get; set; }
        internal int InternalProperty { get; set; }
        public int PublicProperty { get; set; }

        protected override int ProtectedVirtualProperty { get; set; }
        protected internal override int ProtectedInternalVirtualProperty { get; set; }
        internal override int InternalVirtualProperty { get; set; }
        public override int PublicVirtualProperty { get; set; }

        private int PrivateMethod() => 0;

        protected int ProtectedMethod() => 0;

        protected internal int ProtectedInternalMethod() => 0;

        internal int InternalMethod() => 0;

        public int PublicMethod() => 0;

        protected override int ProtectedVirtualMethod() => 0;

        protected internal override int ProtectedInternalVirtualMethod() => 0;

        internal override int InternalVirtualMethod() => 0;

        public override int PublicVirtualMethod() => 0;
    }
}
