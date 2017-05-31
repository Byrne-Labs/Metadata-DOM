using System;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.InheritenceSamples
{
    public class ConcreteSuperClassWithOverrideSample : AbstractSuperClassSample
    {
        public int publicField;
        protected internal int protectedInternalField;
        protected int protectedField;
        internal int internalField;

        public int PublicProperty
        {
            get => protectedInternalField;
            set => protectedInternalField = value;
        }

        public virtual int PublicVirtualProperty
        {
            get => protectedInternalField;
            set => protectedInternalField = value;
        }

        protected internal int ProtectedInternalProperty
        {
            get => protectedField;
            set => protectedField = value;
        }

        protected internal virtual int ProtectedInternalVirtualProperty
        {
            get => protectedField;
            set => protectedField = value;
        }

        protected int ProtectedProperty
        {
            get => internalField;
            set => internalField = value;
        }

        protected virtual int ProtectedVirtualProperty
        {
            get => internalField;
            set => internalField = value;
        }

        internal int InternalProperty { get; set; }

        internal virtual int InternalVirtualProperty
        {
            get => InternalProperty;
            set => InternalProperty = value;
        }

        private int PrivateProperty
        {
            get => publicField;
            set => publicField = value;
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
        public override int PublicAbstractAutoProperty
        {
            get => throw new NotSupportedException("This will be supported in the future");
            set => throw new NotSupportedException("This will be supported in the future");
        }

        protected internal override int ProtectedInternalAbstractAutoProperty
        {
            get => throw new NotSupportedException("This will be supported in the future");
            set => throw new NotSupportedException("This will be supported in the future");
        }

        protected override int ProtectedAbstractAutoProperty
        {
            get => throw new NotSupportedException("This will be supported in the future");
            set => throw new NotSupportedException("This will be supported in the future");
        }

        internal override int InternalAbstractAutoProperty
        {
            get => throw new NotSupportedException("This will be supported in the future");
            set => throw new NotSupportedException("This will be supported in the future");
        }
#elif CSHARP_V3
        public override int PublicAbstractAutoProperty {get;set;}
        protected internal override int ProtectedInternalAbstractAutoProperty {get;set;}
        protected override int ProtectedAbstractAutoProperty {get;set;}
        internal override int InternalAbstractAutoProperty {get;set;}
#endif
        public override int PublicAbstractProperty
        {
            get => publicField;
            set => publicField = value;
        }

        protected internal override int ProtectedInternalAbstractProperty
        {
            get => publicField;
            set => publicField = value;
        }

        protected override int ProtectedAbstractProperty
        {
            get => publicField;
            set => publicField = value;
        }

        internal override int InternalAbstractProperty
        {
            get => publicField;
            set => publicField = value;
        }

        public override int PublicAbstractMethod() => 0;

        public int PublicMethod() => 0;

        public override int PublicVirtualMethod() => 0;

        protected internal override int ProtectedInternalAbstractMethod() => 0;

        protected internal int ProtectedInternalMethod() => 0;

        protected internal override int ProtectedInternalVirtualMethod() => 0;

        protected override int ProtectedAbstractMethod() => 0;

        protected int ProtectedMethod() => 0;

        protected override int ProtectedVirtualMethod() => 0;

        internal override int InternalAbstractMethod() => 0;

        internal int InternalMethod() => 0;

        internal override int InternalVirtualMethod() => 0;

        private int PrivateMethod() => 0;
    }
}
