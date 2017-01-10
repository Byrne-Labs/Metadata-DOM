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
                this.disposing = (EventHandler)Delegate.Combine((Delegate)this.disposing, (Delegate)value);
            }
            remove
            {
                this.disposing = (EventHandler)Delegate.Remove((Delegate)this.disposing, (Delegate)value);
            }
        }

        public event EventHandler Disposing2;

    }
}
