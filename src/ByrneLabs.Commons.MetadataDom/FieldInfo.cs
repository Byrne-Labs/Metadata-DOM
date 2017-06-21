using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class FieldInfo : System.Reflection.FieldInfo, IMemberInfo
    {
        public abstract string FullName { get; }

        public abstract string TextSignature { get; }

        public BindingFlags BindingFlags => TypeInfo.CalculateBindingFlags(IsPublic, IsInherited, IsStatic);

        public virtual bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public bool IsInherited => DeclaringType == ReflectedType;

        public override MemberTypes MemberType => MemberTypes.Field;

        public abstract object GetRawConstantValue();

        public override string ToString() => FullName;
    }
}
