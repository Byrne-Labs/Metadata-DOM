using System;
using System.Collections.Generic;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
#if CSHARP_V2
    public class GenericClass<TAnything1, TAnything2, TItemBase, TClass, TStruct, TNewable, TItem, TEnumerable> where TClass : class where TStruct : struct where TNewable : new() where TItem : TItemBase where TEnumerable : IEnumerable<TItem>
    {
        private class NestedA<TAnything1, TAnything2, TItemBase, TClass, TStruct, TNewable, TItem, TEnumerable> where TClass : class where TStruct : struct where TNewable : new() where TItem : TItemBase where TEnumerable : IEnumerable<TItem>
        {
        }

        private class NestedB
        {
        }

        private class NestedC<TAnything1, TAnything2>
        {
        }

        private class NestedD<TSomethingNew, TAnything2>
        {
        }

        private class NestedE<TSomethingCompletelyNew>
        {
        }

        private class NestedF<TAnything1, TAnything2, TItemBase, TClass, TStruct, TNewable, TItem, TEnumerable, TSomethingCompletelyNew> where TClass : class where TStruct : struct where TNewable : new() where TItem : TItemBase where TEnumerable : IEnumerable<TItem>
        {
        }

        private class NestedG<TAnything1, TAnything2, TItemBase, TClass, TStruct, TNewable, TItem> where TClass : struct where TStruct : class where TNewable : TItemBase where TItem : new() where TAnything2 : IEnumerable<TAnything1>
        {
        }


        public GenericClass(TAnything1 tAnything1)
        {
        }

        public GenericClass(TStruct? tStruct)
        {
        }

        public GenericClass(TStruct[] a)
        {
        }

        public GenericClass(out TimeSpan? a)
        {
            a = null;
        }

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

        public T[] ToArray<T>(T[] a)
        {
            return null;
        }

        public void Something2(TStruct? tStruct)
        {
        }

        public void Something1(out TimeSpan? a)
        {
            a = null;
        }

        public IEnumerable<T> OneLevelEnumerable<T>(IEnumerable<T> a)
        {
            return null;
        }

        public IEnumerable<T> OneLevelEnumerable<T, B>(IEnumerable<T> a)
        {
            return null;
        }

        public IEnumerable<IEnumerable<T>> TwoLevelEnumerable<T>(IEnumerable<IEnumerable<T>> a)
        {
            return null;
        }

        public IEnumerable<IEnumerable<IEnumerable<T>>> ThreeLevelEnumerable<T>(IEnumerable<IEnumerable<IEnumerable<T>>> a)
        {
            return null;
        }

        public IEnumerable<IEnumerable<IEnumerable<IEnumerable<T>>>> FourLevelEnumerable<T>(IEnumerable<IEnumerable<IEnumerable<IEnumerable<T>>>> a)
        {
            return null;
        }

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
