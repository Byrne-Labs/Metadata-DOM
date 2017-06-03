using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using TypeInfoToExpose = System.Reflection.TypeInfo;
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using MethodInfoToExpose = System.Reflection.MethodInfo;
using PropertyInfoToExpose = System.Reflection.PropertyInfo;
using ModuleToExpose = System.Reflection.Module;
using EventInfoToExpose = System.Reflection.EventInfo;
using ParameterInfoToExpose = System.Reflection.ParameterInfo;
using MethodBodyToExpose = System.Reflection.MethodBody;

#else
using TypeInfoToExpose = ByrneLabs.Commons.MetadataDom.TypeInfo;
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using MethodInfoToExpose = ByrneLabs.Commons.MetadataDom.MethodInfo;
using PropertyInfoToExpose = ByrneLabs.Commons.MetadataDom.PropertyInfo;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;
using EventInfoToExpose = ByrneLabs.Commons.MetadataDom.EventInfo;
using ParameterInfoToExpose = ByrneLabs.Commons.MetadataDom.ParameterInfo;
using MethodBodyToExpose = ByrneLabs.Commons.MetadataDom.MethodBody;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class MethodImport : MethodInfo, IManagedCodeElement
    {
        private readonly Lazy<ModuleReference> _module;

        internal MethodImport(System.Reflection.Metadata.MethodImport metadata, MetadataState metadataState)
        {
            Key = new CodeElementKey<MethodImport>(metadata);
            MetadataState = metadataState;
            RawMetadata = metadata;
            MethodImportAttributes = metadata.Attributes;
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            _module = MetadataState.GetLazyCodeElement<ModuleReference>(metadata.Module);
        }

        public override MethodAttributes Attributes => throw NotSupportedHelper.FutureVersion();

        public override Type DeclaringType => throw NotSupportedHelper.FutureVersion();

        public override string FullName => Name;

        public override bool IsCompilerGenerated => throw NotSupportedHelper.FutureVersion();

        public override int MetadataToken => Key.Handle.Value.GetHashCode();

        public override RuntimeMethodHandle MethodHandle => throw NotSupportedHelper.FutureVersion();

        public MethodImportAttributes MethodImportAttributes { get; }

        public override ModuleToExpose Module => _module.Value;

        public override string Name { get; }

        public override IEnumerable<ParameterInfoToExpose> Parameters => throw NotSupportedHelper.FutureVersion();

        public System.Reflection.Metadata.MethodImport RawMetadata { get; }

        public override Type ReflectedType => throw NotSupportedHelper.FutureVersion();

        public override EventInfoToExpose RelatedEvent => throw NotSupportedHelper.FutureVersion();

        public override PropertyInfoToExpose RelatedProperty => throw NotSupportedHelper.FutureVersion();

        public override ParameterInfoToExpose ReturnParameter => throw NotSupportedHelper.FutureVersion();

        public override Type ReturnType => throw NotSupportedHelper.FutureVersion();

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw NotSupportedHelper.FutureVersion();

        public override string TextSignature => Name;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override MethodInfoToExpose GetBaseDefinition() => throw NotSupportedHelper.FutureVersion();

        public override object[] GetCustomAttributes(bool inherit) => throw NotSupportedHelper.FutureVersion();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => throw NotSupportedHelper.FutureVersion();

        public override MethodImplAttributes GetMethodImplementationFlags() => throw NotSupportedHelper.FutureVersion();

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw NotSupportedHelper.FutureVersion();

        public override bool IsDefined(Type attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();
    }
}
