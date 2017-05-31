namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.InheritenceSamples
{
    public abstract class AbstractSuperClassSample : AbstractBaseClassSample
    {
        public int publicField;
        protected internal int protectedInternalField;
        protected int protectedField;
        internal int internalField;

        public int PublicProperty { get; set; }

        public virtual int PublicVirtualProperty
        {
            get => PublicProperty;
            set => PublicProperty = value;
        }

        protected internal int ProtectedInternalProperty
        {
            get => PublicProperty;
            set => PublicProperty = value;
        }

        protected internal virtual int ProtectedInternalVirtualProperty
        {
            get => PublicProperty;
            set => PublicProperty = value;
        }

        protected int ProtectedProperty
        {
            get => PublicProperty;
            set => PublicProperty = value;
        }

        protected virtual int ProtectedVirtualProperty
        {
            get => PublicProperty;
            set => PublicProperty = value;
        }

        internal int InternalProperty
        {
            get => PublicProperty;
            set => PublicProperty = value;
        }

        internal virtual int InternalVirtualProperty
        {
            get => PublicProperty;
            set => PublicProperty = value;
        }

        private int PrivateProperty
        {
            get => PublicProperty;
            set => PublicProperty = value;
        }

        public int PublicMethod() => 0;

        public virtual int PublicVirtualMethod() => 0;

        protected internal int ProtectedInternalMethod() => 0;

        protected internal virtual int ProtectedInternalVirtualMethod() => 0;

        protected int ProtectedMethod() => 0;

        protected virtual int ProtectedVirtualMethod() => 0;

        internal int InternalMethod() => 0;

        internal virtual int InternalVirtualMethod() => 0;

        private int PrivateMethod() => 0;

#if CSHARP_V3
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
    }
}
