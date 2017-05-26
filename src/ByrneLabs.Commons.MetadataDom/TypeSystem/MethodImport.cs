using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using System.Globalization;
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
    //[PublicAPI]
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

        public override MethodAttributes Attributes => throw new NotSupportedException();

        public override Type DeclaringType => throw new NotSupportedException();

        public override string FullName => Name;

        public override bool IsCompilerGenerated => throw new NotSupportedException();

        public override int MetadataToken => RawMetadata.GetHashCode();

        public override RuntimeMethodHandle MethodHandle => throw new NotSupportedException();

        public MethodImportAttributes MethodImportAttributes { get; }

        public override ModuleToExpose Module => _module.Value;

        public override string Name { get; }

        public override IEnumerable<ParameterInfoToExpose> Parameters => throw new NotSupportedException();

        public System.Reflection.Metadata.MethodImport RawMetadata { get; }

        public override Type ReflectedType => throw new NotSupportedException();

        public override EventInfoToExpose RelatedEvent => throw new NotSupportedException();

        public override PropertyInfoToExpose RelatedProperty => throw new NotSupportedException();

        public override ParameterInfoToExpose ReturnParameter => throw new NotSupportedException();

        public override Type ReturnType => throw new NotSupportedException();

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw new NotSupportedException();

        public override string TextSignature => Name;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override MethodInfoToExpose GetBaseDefinition() => throw new NotSupportedException();

        public override object[] GetCustomAttributes(bool inherit) => throw new NotSupportedException();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotSupportedException();

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => throw new NotSupportedException();

        public override MethodImplAttributes GetMethodImplementationFlags() => throw new NotSupportedException();

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw new NotSupportedException();

        public override bool IsDefined(Type attributeType, bool inherit) => throw new NotSupportedException();
    }
}
