using System;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    public class Events
    {
        private EventHandler disposing;

        public event EventHandler Disposing
        {
            add
            {
                disposing = (EventHandler) Delegate.Combine(disposing, value);
            }
            remove
            {
                disposing = (EventHandler) Delegate.Remove(disposing, value);
            }
        }

        public event EventHandler Disposing2;
    }
}
