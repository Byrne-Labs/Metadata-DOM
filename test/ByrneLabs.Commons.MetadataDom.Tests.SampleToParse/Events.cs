﻿using System;

namespace ByrneLabs.Commons.MetadataDom.Tests.SampleToParse
{
    public class Events
    {
        private EventHandler disposing;

        public event EventHandler Disposing
        {
            add => disposing = (EventHandler) Delegate.Combine(disposing, value);
            remove => disposing = (EventHandler) Delegate.Remove(disposing, value);
        }

        public event EventHandler Disposing2;

        public void add_FakeEvent(EventHandler eventHandler)
        {
        }

        public void raise_FakeEvent(EventHandler eventHandler)
        {
        }

        public void remove_FakeEvent(EventHandler eventHandler)
        {
        }
    }
}
