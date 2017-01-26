using System;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.InheritenceSamples
{
    public class ConcreteSuperClassWithOverrideSample : AbstractSuperClassSample
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

#if CSHARP_V7
        public override int PublicAbstractAutoProperty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        protected internal override int ProtectedInternalAbstractAutoProperty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        protected override int ProtectedAbstractAutoProperty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        internal override int InternalAbstractAutoProperty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
#elif CSHARP_V3
        public override int PublicAbstractAutoProperty {get;set;}
        protected internal override int ProtectedInternalAbstractAutoProperty {get;set;}
        protected override int ProtectedAbstractAutoProperty {get;set;}
        internal override int InternalAbstractAutoProperty {get;set;}
#endif
        public override int PublicAbstractProperty
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
        protected internal override int ProtectedInternalAbstractProperty
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
        protected override int ProtectedAbstractProperty
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
        internal override int InternalAbstractProperty
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

        public override int PublicAbstractMethod() { return 0; }

        public int PublicMethod() { return 0; }

        public override int PublicVirtualMethod() { return 0; }

        protected internal override int ProtectedInternalAbstractMethod() { return 0; }

        protected internal int ProtectedInternalMethod() { return 0; }

        protected internal override int ProtectedInternalVirtualMethod() { return 0; }

        protected override int ProtectedAbstractMethod() { return 0; }

        protected int ProtectedMethod() { return 0; }

        protected override int ProtectedVirtualMethod() { return 0; }

        internal override int InternalAbstractMethod() { return 0; }

        internal int InternalMethod() { return 0; }

        internal override int InternalVirtualMethod() { return 0; }

        private int PrivateMethod() { return 0; }
    }
}
