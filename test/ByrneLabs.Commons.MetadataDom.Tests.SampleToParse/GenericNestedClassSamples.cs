using System;
using System.Collections.Generic;
using System.Text;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
#if CSHARP_V2

    public class GenericLevel1<T>
    {
        public T value;

        public T Value
        {
            get
            {
                return default(T);
            }
        }

        public void DoSomething()
        {
        }

        public class GenericLevel2<T> : GenericInterfaceLevel1<T>
        {
            public T value;

            public void DoSomething()
            {
            }

            public T Value
            {
                get
                {
                    return default(T);
                }
            }

            public class GenericLevel3 : GenericLevel1<T>
            {
                public T value;

                public T Value
                {
                    get
                    {
                        return default(T);
                    }
                }

                public void DoSomething()
                {
                }

                public class GenericLevel4 : GenericInterfaceLevel1<T>
                {
                    public T value;

                    public void DoSomething()
                    {
                    }

                    public T Value
                    {
                        get
                        {
                            return default(T);
                        }
                    }

                    public class GenericLevel5<T>
                    {
                        public T value;

                        public void DoSomething()
                        {
                        }

                        public T Value
                        {
                            get
                            {
                                return default(T);
                            }
                        }
                    }

                }

                public interface GenericInterfaceLevel1<T>
                {
                    T Value { get; }

                    void DoSomething();
                }
            }
        }

        public interface GenericInterfaceLevel1<T>
        {
            T Value { get; }

            void DoSomething();
        }
    }
#endif
}
