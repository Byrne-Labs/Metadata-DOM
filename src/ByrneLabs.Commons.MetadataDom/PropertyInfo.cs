using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class PropertyInfo : System.Reflection.PropertyInfo, IMemberInfo
    {
        public abstract ConstantInfo DefaultValue { get; }

        public abstract string FullName { get; }

        public abstract string FullTextSignature { get; }

        public abstract bool IsIndexer { get; }

        public abstract IEnumerable<SequencePoint> SequencePoints { get; }

        public abstract string SourceCode { get; }

        public abstract string TextSignature { get; }

        public BindingFlags BindingFlags => TypeInfo.CalculateBindingFlags(IsPublic, IsInherited, IsStatic);

        public bool IsAbstract => GetMethod?.IsAbstract == true || SetMethod?.IsAbstract == true;

        public virtual bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public bool IsInherited => DeclaringType == ReflectedType;

        public bool IsPublic => GetMethod?.IsPublic != false && SetMethod?.IsPublic != false;

        public bool IsStatic => GetMethod?.IsStatic != false && SetMethod?.IsStatic != false;

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) => throw NotSupportedHelper.FutureVersion();

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture) => throw NotSupportedHelper.FutureVersion();

        public override string ToString() => FullName;
    }
}
