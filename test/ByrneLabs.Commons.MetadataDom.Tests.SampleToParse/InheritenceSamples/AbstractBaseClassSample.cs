namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.InheritenceSamples
{
    public abstract class AbstractBaseClassSample
    {
        public int publicField;
        protected internal int protectedInternalField;
        protected int protectedField;
        internal int internalField;
        private int privateField;

        public abstract int PublicAbstractProperty { get; set; }

        protected internal abstract int ProtectedInternalAbstractProperty { get; set; }

        protected abstract int ProtectedAbstractProperty { get; set; }

        internal abstract int InternalAbstractProperty { get; set; }

        public int PublicProperty { get; set; }

        public virtual int PublicVirtualProperty { get; set; }

        protected internal int ProtectedInternalProperty { get; set; }

        protected internal virtual int ProtectedInternalVirtualProperty { get; set; }

        protected int ProtectedProperty { get; set; }

        protected virtual int ProtectedVirtualProperty { get; set; }

        internal int InternalProperty { get; set; }

        internal virtual int InternalVirtualProperty { get; set; }

        private int PrivateProperty { get; set; }

        public abstract int PublicAbstractMethod();

        protected internal abstract int ProtectedInternalAbstractMethod();

        protected abstract int ProtectedAbstractMethod();

        internal abstract int InternalAbstractMethod();

        public int PublicMethod() => 0;

        public virtual int PublicVirtualMethod() => 0;

        protected internal int ProtectedInternalMethod() => 0;

        protected internal virtual int ProtectedInternalVirtualMethod() => 0;

        protected int ProtectedMethod() => 0;

        protected virtual int ProtectedVirtualMethod() => 0;

        internal int InternalMethod() => 0;

        internal virtual int InternalVirtualMethod() => 0;

        private int PrivateMethod() => 0;
    }
}
