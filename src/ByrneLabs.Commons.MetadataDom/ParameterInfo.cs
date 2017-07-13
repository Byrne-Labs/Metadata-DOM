using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class ParameterInfo : System.Reflection.ParameterInfo, IMemberInfo
    {
        public abstract string FullName { get; }

        public abstract bool IsCompilerGenerated { get; }

        public abstract bool IsSpecialName { get; }

        public abstract System.Reflection.Module Module { get; }

        public abstract IEnumerable<SequencePoint> SequencePoints { get; }

        public abstract string SourceCode { get; }

        public BindingFlags BindingFlags => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public virtual string FullTextSignature => TextSignatureImpl();

        public MemberTypes MemberType => MemberTypes.Custom;

        public override object RawDefaultValue => throw NotSupportedHelper.FutureVersion();

        public virtual string TextSignature => FullTextSignature;

        public override string ToString() => $"{ParameterType} {Name}";

        private string TextSignatureImpl() => ParameterType == null ? string.Empty : (IsOut ? "out " : string.Empty) + ParameterType.FullName + " " + Name;
    }
}
