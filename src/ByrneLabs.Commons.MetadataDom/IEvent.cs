using System;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    public interface IEvent : IMember
    {
        IMethod AddMethod { get; }

        EventAttributes Attributes { get; }

        IType EventHandlerType { get; }

        bool IsMulticast { get; }

        bool IsSpecialName { get; }

        IMethod RaiseMethod { get; }

        IMethod RemoveMethod { get; }
    }
}
