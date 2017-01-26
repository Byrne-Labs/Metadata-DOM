using System.Collections.Generic;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
#if CSHARP_V2
    public class GenericClass<TAnything, TClass, TStruct, TNewable, TItem, TEnumerable> where TClass : class where TStruct : struct where TNewable : new() where TItem : TAnything where TEnumerable : IEnumerable<TItem>
    {
#if CSHARP_V3
        public TAnything TAnythingValueAutoProperty { get; set; }

        public TClass TClassValueAutoProperty { get; set; }

        public TEnumerable TEnumerableValueAutoProperty { get; set; }

        public TItem TItemValueAutoProperty { get; set; }

        public TNewable TNewableValueAutoProperty { get; set; }

        public TStruct TStructValueAutoProperty { get; set; }
#endif

        public TAnything TAnythingValue { get { return default(TAnything); } set {; } }

        public TClass TClassValue { get { return default(TClass); } set {; } }

        public TEnumerable TEnumerableValue { get { return default(TEnumerable); } set {; } }

        public TItem TItemValue { get { return default(TItem); } set {; } }

        public TNewable TNewableValue { get { return default(TNewable); } set {; } }

        public TStruct TStructValue { get { return default(TStruct); } set {; } }

    }
#endif
}
