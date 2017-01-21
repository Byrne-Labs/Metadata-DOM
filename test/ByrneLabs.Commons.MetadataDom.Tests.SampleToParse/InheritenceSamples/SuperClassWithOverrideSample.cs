namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.InheritenceSamples
{
    public class SuperClassWithOverrideSample : BaseClassSample
    {
        public int publicField;
        protected internal int protectedInternalField;
        protected int protectedField;
        internal int internalField;
        private int privateField;

        public int PublicProperty { get; set; }

        public override int PublicVirtualProperty { get; set; }

        protected internal int ProtectedInternalProperty { get; set; }

        protected internal override int ProtectedInternalVirtualProperty { get; set; }

        protected int ProtectedProperty { get; set; }

        protected override int ProtectedVirtualProperty { get; set; }

        internal int InternalProperty { get; set; }

        internal override int InternalVirtualProperty { get; set; }

        private int PrivateProperty { get; set; }

        public int PublicMethod() => 0;

        public override int PublicVirtualMethod() => 0;

        protected internal int ProtectedInternalMethod() => 0;

        protected internal override int ProtectedInternalVirtualMethod() => 0;

        protected int ProtectedMethod() => 0;

        protected override int ProtectedVirtualMethod() => 0;

        internal int InternalMethod() => 0;

        internal override int InternalVirtualMethod() => 0;

        private int PrivateMethod() => 0;
    }
}
