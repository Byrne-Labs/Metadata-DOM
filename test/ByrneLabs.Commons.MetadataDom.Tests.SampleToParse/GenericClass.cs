using System.Collections.Generic;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    public class GenericClass<TAnything, TClass, TStruct, TNewable, TItem, TEnumerable> where TClass : class where TStruct : struct where TNewable : new() where TItem : TAnything where TEnumerable : IEnumerable<TItem>
    {
        public TAnything TAnythingValue { get; set; }

        public TClass TClassValue { get; set; }

        public TEnumerable TEnumerableValue { get; set; }

        public TItem TItemValue { get; set; }

        public TNewable TNewableValue { get; set; }

        public TStruct TStructValue { get; set; }
    }
}
