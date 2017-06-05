﻿using System.Reflection;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using System.Collections.Generic;
using System.Linq;
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using ModuleToExpose = System.Reflection.Module;
using ParameterInfoToExpose = System.Reflection.ParameterInfo;

#else
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;
using ParameterInfoToExpose = ByrneLabs.Commons.MetadataDom.ParameterInfo;

#endif

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract partial class ConstructorInfo
    {
        public BindingFlags BindingFlags => TypeInfo.CalculateBindingFlags(IsPublic, IsInherited, IsStatic);

        public override bool IsSecurityCritical => throw NotSupportedHelper.NotValidForMetadata();

        public override bool IsSecuritySafeCritical => throw NotSupportedHelper.NotValidForMetadata();

        public override bool IsSecurityTransparent => throw NotSupportedHelper.NotValidForMetadata();

        public override string ToString() => TextSignature;
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public abstract partial class ConstructorInfo : System.Reflection.ConstructorInfo, IMemberInfo
    {
        public abstract string FullName { get; }

        public abstract IEnumerable<ParameterInfoToExpose> Parameters { get; }

        public abstract string TextSignature { get; }

        public virtual bool IsCompilerGenerated => CustomAttributes.Any(customAttribute => "System.Runtime.CompilerServices.CompilerGeneratedAttribute".Equals(customAttribute.Constructor.DeclaringType.Name));

        public bool IsInherited => DeclaringType == ReflectedType;

        public override ParameterInfoToExpose[] GetParameters() => Parameters.ToArray();
    }
#else
    public abstract partial class ConstructorInfo : MethodBase
    {
    }
#endif
}
