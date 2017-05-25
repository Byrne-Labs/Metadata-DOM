using System;
using System.Reflection;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System;
using MethodBaseToExpose = System.Reflection.MethodBase;
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using MethodInfoToExpose = System.Reflection.MethodInfo;
using ModuleToExpose = System.Reflection.Module;
using MemberInfoToExpose = System.Reflection.MemberInfo;

#else
using MethodBaseToExpose = ByrneLabs.Commons.MetadataDom.MethodBase;
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using MethodInfoToExpose = ByrneLabs.Commons.MetadataDom.MethodInfo;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;
using MemberInfoToExpose = ByrneLabs.Commons.MetadataDom.MemberInfo;

#endif

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract partial class ParameterInfo
    {
        public abstract ParameterAttributes Attributes { get; }

        public abstract object DefaultValue { get; }

        public abstract bool HasDefaultValue { get; }

        public abstract MemberInfoToExpose Member { get; }

        public abstract TypeToExpose ParameterType { get; }

        public abstract int Position { get; }

        public bool IsIn => (Attributes & ParameterAttributes.In) != 0;

        public bool IsLcid => (Attributes & ParameterAttributes.Lcid) != 0;

        public bool IsOptional => (Attributes & ParameterAttributes.Optional) != 0;

        public bool IsOut => (Attributes & ParameterAttributes.Out) != 0;

        public bool IsRetval => (Attributes & ParameterAttributes.Retval) != 0;

        private string TextSignatureImpl() => ParameterType == null ? string.Empty : (IsOut ? "out " : string.Empty) + ParameterType.FullName + " " + Name;
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public abstract partial class ParameterInfo : System.Reflection.ParameterInfo, IMemberInfo
    {

        public abstract string FullName { get; }

        public abstract bool IsSpecialName { get; }

        public abstract int MetadataToken { get; }

        public abstract ModuleToExpose Module { get; }

        public abstract string Name { get; }

        public MemberTypes MemberType => MemberTypes.Custom;

        public abstract bool IsCompilerGenerated { get; }

        public abstract IList<CustomAttributeDataToExpose> GetCustomAttributesData();

        public virtual string TextSignature => TextSignatureImpl();

        [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "False positive because it is not picking up the PublicAPI attribute on the other part of the partial class")]
        public virtual object RawDefaultValue => throw new NotSupportedException();
    }
#else
    public abstract partial class ParameterInfo : MemberInfo
    {
        public override TypeToExpose DeclaringType => Member as TypeToExpose ?? Member.DeclaringType;

        public override MemberTypes MemberType => MemberTypes.Custom;

        public override Type ReflectedType => throw new NotSupportedException();

        public override string TextSignature => TextSignatureImpl();
    }
#endif
}
