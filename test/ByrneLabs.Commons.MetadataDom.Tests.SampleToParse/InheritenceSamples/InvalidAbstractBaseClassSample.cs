namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.InheritenceSamples
{
    public abstract class InvalidAbstractBaseClassSample
    {
#if CSHARP_V3
        internal abstract int InternalAbstractAutoProperty { get; set; }
#endif
        internal abstract int InternalAbstractProperty { get; set; }

        internal abstract int InternalAbstractMethod();
    }
}
