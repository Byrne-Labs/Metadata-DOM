using System;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.InheritenceSamples
{
    public abstract class InvalidAbstractSuperClassSample : InvalidAbstractBaseClassSample
    {
        internal abstract int InternalAbstractProperty { get; set; }

        internal abstract int InternalAbstractMethod();

    }

}
