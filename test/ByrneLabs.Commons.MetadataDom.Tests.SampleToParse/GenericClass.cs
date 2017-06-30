using System;
using System.Collections.Generic;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
#if CSHARP_V2

    internal class NonGenericClass
    {
        internal delegate int CountGetter<TEntity>(TEntity entity, out int count);

        internal delegate int CountGetter<TEntity, TEntity2>(TEntity entity, out int count);

        internal delegate int ItemsGetter<TEntity, TItem>(TEntity entity, int bufferLength, out int count, TItem[] buffer);

        public void Something(out int?[] a)
        {
            a = null;
        }

        internal static TItem[] GetItems<TEntity, TItem>(TEntity entity, CountGetter<TEntity> countGetter, ItemsGetter<TEntity, TItem> itemsGetter) => null;

        private int GenericCount<TEntity>(TEntity entity) => 0;

        private int GenericCount<TEntity, TEntity2>(TEntity entity) => 0;
    }

    public class GenericClass<TAnything1, TAnything2, TItemBase, TClass, TStruct, TNewable, TItem, TEnumerable> where TClass : class where TStruct : struct where TNewable : new() where TItem : TItemBase where TEnumerable : IEnumerable<TItem>
    {
        internal delegate void SomethingDelegate(TAnything1 a);

        internal delegate void SomethingDelegateA(object a);

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

        private Func<TAnything1> getValue;

        public GenericClass(TAnything1 tAnything1)
        {
        }

        public GenericClass(TStruct? tStruct)
        {
        }

        public GenericClass(TStruct[] a)
        {
        }

        public GenericClass(Action<IEnumerable<IEnumerable<TAnything1>>, IEnumerable<TAnything1>, TAnything1> a, TAnything1 b)
        {
        }

        public GenericClass(out TimeSpan? a)
        {
            a = null;
        }

        public TAnything1 TAnything1Value
        {
            get => default(TAnything1);
            set
            {
                ;
            }
        }

        public TAnything2 TAnything2Value
        {
            get => default(TAnything2);
            set
            {
                ;
            }
        }

        public TClass TClassValue
        {
            get => default(TClass);
            set
            {
                ;
            }
        }

        public TEnumerable TEnumerableValue
        {
            get => default(TEnumerable);
            set
            {
                ;
            }
        }

        public TItemBase TItemBaseValue
        {
            get => default(TItemBase);
            set
            {
                ;
            }
        }

        public TItem TItemValue
        {
            get => default(TItem);
            set
            {
                ;
            }
        }

        public TNewable TNewableValue
        {
            get => default(TNewable);
            set
            {
                ;
            }
        }

        public TStruct TStructValue
        {
            get => default(TStruct);
            set
            {
                ;
            }
        }

        public IEnumerable<IEnumerable<IEnumerable<IEnumerable<T>>>> FourLevelEnumerable<T>(IEnumerable<IEnumerable<IEnumerable<IEnumerable<T>>>> a) => null;

        public IEnumerable<T> OneLevelEnumerable<T>(IEnumerable<T> a) => null;

        public IEnumerable<T> OneLevelEnumerable<T, B>(IEnumerable<T> a) => null;

        public void Something(Action<IEnumerable<IEnumerable<TAnything1>>, IEnumerable<TAnything1>, TAnything1> a, TAnything1 b)
        {
        }

        public void Something1(out TimeSpan? a)
        {
            a = null;
        }

        public void Something2(TStruct? tStruct)
        {
        }

        public IEnumerable<IEnumerable<IEnumerable<T>>> ThreeLevelEnumerable<T>(IEnumerable<IEnumerable<IEnumerable<T>>> a) => null;

        public T[] ToArray<T>(T[] a) => null;

        public T[] ToArray<T, B>(T[] a) => null;

        public IEnumerable<IEnumerable<T>> TwoLevelEnumerable<T>(IEnumerable<IEnumerable<T>> a) => null;

        internal static void SomethingA(IEnumerable<TAnything1> a, TAnything1 b, SomethingDelegate c)
        {
        }

        internal static void SomethingB(IEnumerable<TAnything1> a, TAnything1 b, SomethingDelegate c)
        {
        }

        internal static void SomethingB(SomethingDelegateA[] a, SomethingDelegate[] b)
        {
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
    }
#endif
}
