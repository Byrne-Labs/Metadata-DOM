﻿using System.Reflection;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using System.Diagnostics.CodeAnalysis;
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
        public BindingFlags BindingFlags => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public bool IsIn => (Attributes & ParameterAttributes.In) != 0;

        public bool IsLcid => (Attributes & ParameterAttributes.Lcid) != 0;

        public bool IsOptional => (Attributes & ParameterAttributes.Optional) != 0;

        public bool IsOut => (Attributes & ParameterAttributes.Out) != 0;

        public bool IsRetval => (Attributes & ParameterAttributes.Retval) != 0;

        public override string ToString() => $"({GetType().FullName}) {TextSignature}";

        private string TextSignatureImpl() => ParameterType == null ? string.Empty : (IsOut ? "out " : string.Empty) + ParameterType.FullName + " " + Name;
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public abstract partial class ParameterInfo : System.Reflection.ParameterInfo, IMemberInfo
    {
        public abstract string FullName { get; }

        public abstract bool IsCompilerGenerated { get; }

        public abstract bool IsSpecialName { get; }

        public abstract ModuleToExpose Module { get; }

        public MemberTypes MemberType => MemberTypes.Custom;

        [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "False positive because it is not picking up the PublicAPI attribute on the other part of the partial class")]
        public virtual object RawDefaultValue => throw NotSupportedHelper.FutureVersion();

        public virtual string TextSignature => TextSignatureImpl();
    }
#else
    public abstract partial class ParameterInfo : MemberInfo
    {
        public abstract ParameterAttributes Attributes { get; }

        public abstract object DefaultValue { get; }

        public abstract bool HasDefaultValue { get; }

        public abstract MemberInfoToExpose Member { get; }

        public abstract TypeToExpose ParameterType { get; }

        public abstract int Position { get; }

        public override TypeToExpose DeclaringType => Member as TypeToExpose ?? Member.DeclaringType;

        public override MemberTypes MemberType => MemberTypes.Custom;

        public override Type ReflectedType => throw NotSupportedHelper.FutureVersion();

        public override string TextSignature => TextSignatureImpl();
    }
#endif
}
