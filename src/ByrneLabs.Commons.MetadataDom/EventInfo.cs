using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class EventInfo : System.Reflection.EventInfo, IMemberInfo
    {
        public abstract string FullName { get; }

        public abstract IEnumerable<SequencePoint> SequencePoints { get; }

        public abstract string SourceCode { get; }

        public abstract string TextSignature { get; }

        public BindingFlags BindingFlags => TypeInfo.CalculateBindingFlags(IsPublic, IsInherited, IsStatic);

        public virtual bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public bool IsInherited => DeclaringType == ReflectedType;

        public bool IsPublic => AddMethod?.IsPublic != false && RemoveMethod?.IsPublic != false && RaiseMethod?.IsPublic != false;

        public bool IsStatic => AddMethod?.IsStatic != false && RemoveMethod?.IsStatic != false && RaiseMethod?.IsStatic != false;

        public override string ToString() => FullName;
    }
}
