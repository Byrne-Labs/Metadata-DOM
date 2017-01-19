using System;
using System.Collections.Generic;
using System.Text;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.InheritenceSamples
{
    public class ConcreteSuperClassWithNewSample : AbstractSuperClassSample
    {
        public int publicField;
        protected internal int protectedInternalField;
        protected int protectedField;
        internal int internalField;
        private int privateField;

        public override int PublicAbstractProperty { get; set; }

        public new int PublicProperty { get; set; }

        public new virtual int PublicVirtualProperty { get; set; }

        protected internal override int ProtectedInternalAbstractProperty { get; set; }

        protected internal new int ProtectedInternalProperty { get; set; }

        protected internal new virtual int ProtectedInternalVirtualProperty { get; set; }

        protected override int ProtectedAbstractProperty { get; set; }

        protected new int ProtectedProperty { get; set; }

        protected new virtual int ProtectedVirtualProperty { get; set; }

        internal override int InternalAbstractProperty { get; set; }

        internal new int InternalProperty { get; set; }

        internal new virtual int InternalVirtualProperty { get; set; }

        private int PrivateProperty { get; set; }

        public override int PublicAbstractMethod() => 0;

        public new int PublicMethod() => 0;

        public new virtual int PublicVirtualMethod() => 0;

        protected internal override int ProtectedInternalAbstractMethod() => 0;

        protected internal new int ProtectedInternalMethod() => 0;

        protected internal new virtual int ProtectedInternalVirtualMethod() => 0;

        protected override int ProtectedAbstractMethod() => 0;

        protected new int ProtectedMethod() => 0;

        protected new virtual int ProtectedVirtualMethod() => 0;

        internal override int InternalAbstractMethod() => 0;

        internal new int InternalMethod() => 0;

        internal new virtual int InternalVirtualMethod() => 0;

        private int PrivateMethod() => 0;
    }
}
