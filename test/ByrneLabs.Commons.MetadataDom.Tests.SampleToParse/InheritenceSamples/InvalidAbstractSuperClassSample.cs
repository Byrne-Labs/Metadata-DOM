namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.InheritenceSamples
{
    public abstract class InvalidAbstractSuperClassSample : InvalidAbstractBaseClassSample
    {

        internal abstract int InternalAbstractProperty { get; set; }

#if CSHARP_V3
        internal abstract int InternalAbstractAutoProperty { get; set; }
#endif

        internal abstract int InternalAbstractMethod();
    }
}
