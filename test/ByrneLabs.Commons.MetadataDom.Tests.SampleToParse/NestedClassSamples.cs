using System;
using System.Collections.Generic;
using System.Text;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    public class Level1
    {
        public object value;

        public object Value
        {
            get
            {
                return null;
            }
        }

        public void DoSomething()
        {
        }

        private class Level2 : InterfaceLevel1
        {
            public object value;

            public void DoSomething()
            {
            }

            public object Value
            {
                get
                {
                    return null;
                }
            }

            private class Level3
            {
                public object value;

                public object Value
                {
                    get
                    {
                        return null;
                    }
                }

                public void DoSomething()
                {
                }

                private class Level4 : InterfaceLevel1
                {
                    public object value;

                    public void DoSomething()
                    {
                    }

                    public object Value
                    {
                        get
                        {
                            return null;
                        }
                    }
                }

                private interface InterfaceLevel1
                {
                    object Value { get; }

                    void DoSomething();
                }
            }
        }

        private interface InterfaceLevel1
        {
            object Value { get; }

            void DoSomething();
        }
    }
}
