namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.InheritenceSamples
{
    public class SuperClassSample : BaseClassSample
    {
        public int publicField;
        protected internal int protectedInternalField;
        protected int protectedField;
        internal int internalField;
        private int privateField;

        public int PublicProperty
        {
            get
            {
                return protectedInternalField;
            }
            set
            {
                protectedInternalField = value;
            }
        }

        public virtual int PublicVirtualProperty
        {
            get
            {
                return protectedInternalField;
            }
            set
            {
                protectedInternalField = value;
            }
        }

        protected internal int ProtectedInternalProperty
        {
            get
            {
                return protectedField;
            }
            set
            {
                protectedField = value;
            }
        }

        protected internal virtual int ProtectedInternalVirtualProperty
        {
            get
            {
                return protectedField;
            }
            set
            {
                protectedField = value;
            }
        }

        protected int ProtectedProperty
        {
            get
            {
                return internalField;
            }
            set
            {
                internalField = value;
            }
        }

        protected virtual int ProtectedVirtualProperty
        {
            get
            {
                return internalField;
            }
            set
            {
                internalField = value;
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
                return publicField;
            }
            set
            {
                publicField = value;
            }
        }

#if CSHARP_V3
        public int PublicAutoProperty { get; set; }

        public override int PublicVirtualAutoProperty { get; set; }

        protected internal int ProtectedInternalAutoProperty { get; set; }

        protected internal override int ProtectedInternalVirtualAutoProperty { get; set; }

        protected int ProtectedAutoProperty { get; set; }

        protected override int ProtectedVirtualAutoProperty { get; set; }

        internal int InternalAutoProperty { get; set; }

        internal override int InternalVirtualAutoProperty { get; set; }

        private int PrivateAutoProperty { get; set; }
#endif

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
