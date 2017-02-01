using System.Collections.Generic;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
#if CSHARP_V2
    public class GenericClass<TAnything1, TAnything2, TItemBase, TClass, TStruct, TNewable, TItem, TEnumerable> where TClass : class where TStruct : struct where TNewable : new() where TItem : TItemBase where TEnumerable : IEnumerable<TItem>
    {
#if CSHARP_V3
        public TAnything1 TAnything1ValueAutoProperty { get; set; }

        public TAnything2 TAnything2ValueAutoProperty { get; set; }

        public TItemBase TItemBasValueeAutoProperty { get; set; }

        public TClass TClassValueAutoProperty { get; set; }

        public TEnumerable TEnumerableValueAutoProperty { get; set; }

        public TItem TItemValueAutoProperty { get; set; }

        public TNewable TNewableValueAutoProperty { get; set; }

        public TStruct TStructValueAutoProperty { get; set; }
#endif

        public TAnything1 TAnything1Value { get { return default(TAnything1); } set {; } }

        public TAnything2 TAnything2Value { get { return default(TAnything2); } set {; } }

        public TItemBase TItemBaseValue { get { return default(TItemBase); } set {; } }

        public TClass TClassValue { get { return default(TClass); } set {; } }

        public TEnumerable TEnumerableValue { get { return default(TEnumerable); } set {; } }

        public TItem TItemValue { get { return default(TItem); } set {; } }

        public TNewable TNewableValue { get { return default(TNewable); } set {; } }

        public TStruct TStructValue { get { return default(TStruct); } set {; } }

    }
#endif
}
