using System.Diagnostics.CodeAnalysis;
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

        public BindingFlags BindingFlags => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public bool IsIn => (Attributes & ParameterAttributes.In) != 0;

        public bool IsLcid => (Attributes & ParameterAttributes.Lcid) != 0;

        public bool IsOptional => (Attributes & ParameterAttributes.Optional) != 0;

        public bool IsOut => (Attributes & ParameterAttributes.Out) != 0;

        public bool IsRetval => (Attributes & ParameterAttributes.Retval) != 0;

        public MemberTypes MemberType => MemberTypes.Custom;

        [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "False positive because it is not picking up the PublicAPI attribute on the other part of the class")]
        public virtual object RawDefaultValue => throw NotSupportedHelper.FutureVersion();

        public virtual string TextSignature => TextSignatureImpl();

        public override string ToString() => $"{ParameterType} {Name}";

        private string TextSignatureImpl() => ParameterType == null ? string.Empty : (IsOut ? "out " : string.Empty) + ParameterType.FullName + " " + Name;
    }
}
