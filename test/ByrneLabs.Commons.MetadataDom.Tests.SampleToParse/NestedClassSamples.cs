namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    public class Level1
    {
        private interface InterfaceLevel1
        {
            object Value { get; }

            void DoSomething();
        }

        private class Level2 : InterfaceLevel1
        {
            private class Level3
            {
                private interface InterfaceLevel1
                {
                    object Value { get; }

                    void DoSomething();
                }

                private class Level4 : InterfaceLevel1
                {
                    public object value;

                    public void DoSomething()
                    {
                    }

                    public object Value => null;
                }

                public object value;

                public object Value => null;

                public void DoSomething()
                {
                }
            }

            public object value;

            public void DoSomething()
            {
            }

            public object Value => null;
        }

        public object value;

        public object Value => null;

        public void DoSomething()
        {
        }
    }
}
