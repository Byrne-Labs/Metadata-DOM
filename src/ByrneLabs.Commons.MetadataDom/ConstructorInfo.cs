using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class ConstructorInfo : System.Reflection.ConstructorInfo, IMemberInfo
    {
        public abstract MethodDebugInformation DebugInformation { get; }

        public abstract string FullName { get; }

        public abstract string FullTextSignature { get; }

        public abstract IEnumerable<System.Reflection.ParameterInfo> Parameters { get; }

        public abstract IEnumerable<SequencePoint> SequencePoints { get; }

        public abstract string SourceCode { get; }

        public abstract string TextSignature { get; }

        public BindingFlags BindingFlags => TypeInfo.CalculateBindingFlags(IsPublic, IsInherited, IsStatic);

        public virtual bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public bool IsInherited => DeclaringType == ReflectedType;

        public override bool IsSecurityCritical => throw NotSupportedHelper.NotValidForMetadata();

        public override bool IsSecuritySafeCritical => throw NotSupportedHelper.NotValidForMetadata();

        public override bool IsSecurityTransparent => throw NotSupportedHelper.NotValidForMetadata();

        public override System.Reflection.ParameterInfo[] GetParameters() => Parameters.ToArray();

        public override string ToString() => FullTextSignature;
    }
}
