using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

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

        public override RuntimeMethodHandle MethodHandle => throw new NotSupportedException();

        public MethodImportAttributes MethodImportAttributes { get; }

        public ModuleReference Module => _module.Value;

        public override string Name { get; }

        public override IEnumerable<System.Reflection.ParameterInfo> Parameters => throw new NotSupportedException();

        public System.Reflection.Metadata.MethodImport RawMetadata { get; }

        public override Type ReflectedType => throw new NotSupportedException();

        public override System.Reflection.EventInfo RelatedEvent => throw new NotSupportedException();

        public override System.Reflection.PropertyInfo RelatedProperty => throw new NotSupportedException();

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw new NotSupportedException();

        public override string TextSignature => Name;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override System.Reflection.MethodInfo GetBaseDefinition() => throw new NotSupportedException();

        public override object[] GetCustomAttributes(bool inherit) => throw new NotSupportedException();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw new NotSupportedException();

        public override MethodImplAttributes GetMethodImplementationFlags() => throw new NotSupportedException();

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw new NotSupportedException();

        public override bool IsDefined(Type attributeType, bool inherit) => throw new NotSupportedException();
    }
}
