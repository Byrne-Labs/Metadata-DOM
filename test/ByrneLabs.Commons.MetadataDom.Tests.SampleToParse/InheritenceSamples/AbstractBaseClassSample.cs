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

        public int PublicProperty
        {
            get
            {
                return privateField;
            }
            set
            {
                privateField = value;
            }
        }

        public virtual int PublicVirtualProperty
        {
            get
            {
                return privateField;
            }
            set
            {
                privateField = value;
            }
        }

        protected internal int ProtectedInternalProperty
        {
            get
            {
                return privateField;
            }
            set
            {
                privateField = value;
            }
        }

        protected internal virtual int ProtectedInternalVirtualProperty
        {
            get
            {
                return privateField;
            }
            set
            {
                privateField = value;
            }
        }

        protected int ProtectedProperty
        {
            get
            {
                return privateField;
            }
            set
            {
                privateField = value;
            }
        }

        protected virtual int ProtectedVirtualProperty
        {
            get
            {
                return privateField;
            }
            set
            {
                privateField = value;
            }
        }

        internal int InternalProperty
        {
            get
            {
                return privateField;
            }
            set
            {
                privateField = value;
            }
        }

        internal virtual int InternalVirtualProperty
        {
            get
            {
                return privateField;
            }
            set
            {
                privateField = value;
            }
        }

        private int PrivateProperty
        {
            get
            {
                return privateField;
            }
            set
            {
                privateField = value;
            }
        }
#if CSHARP_V3
        public abstract int PublicAbstractAutoProperty { get; set; }

        protected internal abstract int ProtectedInternalAbstractAutoProperty { get; set; }

        protected abstract int ProtectedAbstractAutoProperty { get; set; }

        internal abstract int InternalAbstractAutoProperty { get; set; }

        public int PublicAutoProperty { get; set; }

        public virtual int PublicVirtualAutoProperty { get; set; }

        protected internal int ProtectedInternalAutoProperty { get; set; }

        protected internal virtual int ProtectedInternalVirtualAutoProperty { get; set; }

        protected int ProtectedAutoProperty { get; set; }

        protected virtual int ProtectedVirtualAutoProperty { get; set; }

        internal int InternalAutoProperty { get; set; }

        internal virtual int InternalVirtualAutoProperty { get; set; }

        private int PrivateAutoProperty { get; set; }
#endif

        public abstract int PublicAbstractMethod();

        protected internal abstract int ProtectedInternalAbstractMethod();

        protected abstract int ProtectedAbstractMethod();

        internal abstract int InternalAbstractMethod();

        public int PublicMethod() { return 0; }

        public virtual int PublicVirtualMethod() { return 0; }

        protected internal int ProtectedInternalMethod() { return 0; }

        protected internal virtual int ProtectedInternalVirtualMethod() { return 0; }

        protected int ProtectedMethod() { return 0; }

        protected virtual int ProtectedVirtualMethod() { return 0; }

        internal int InternalMethod() { return 0; }

        internal virtual int InternalVirtualMethod() { return 0; }

        private int PrivateMethod() { return 0; }
    }
}
