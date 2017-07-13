using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using JetBrains.Annotations;

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

        public override MetadataDom.MethodDebugInformation DebugInformation => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override Type DeclaringType => throw NotSupportedHelper.FutureVersion();

        public override string FullName => Name;

        public override string FullTextSignature => Name;

        public override bool IsCompilerGenerated => throw NotSupportedHelper.FutureVersion();

        public override int MetadataToken => 0;

        public override RuntimeMethodHandle MethodHandle => throw NotSupportedHelper.FutureVersion();

        public MethodImportAttributes MethodImportAttributes { get; }

        public override System.Reflection.Module Module => _module.Value;

        public override string Name { get; }

        public override IEnumerable<System.Reflection.ParameterInfo> Parameters => throw NotSupportedHelper.FutureVersion();

        public System.Reflection.Metadata.MethodImport RawMetadata { get; }

        public override Type ReflectedType => throw NotSupportedHelper.FutureVersion();

        public override EventInfo RelatedEvent => throw NotSupportedHelper.FutureVersion();

        public override PropertyInfo RelatedProperty => throw NotSupportedHelper.FutureVersion();

        public override System.Reflection.ParameterInfo ReturnParameter => throw NotSupportedHelper.FutureVersion();

        public override Type ReturnType => throw NotSupportedHelper.FutureVersion();

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => throw NotSupportedHelper.FutureVersion();

        public override IEnumerable<MetadataDom.SequencePoint> SequencePoints => null;

        public override string SourceCode => null;

        public override string TextSignature => Name;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        public override System.Reflection.MethodInfo GetBaseDefinition() => throw NotSupportedHelper.FutureVersion();

        public override object[] GetCustomAttributes(bool inherit) => throw NotSupportedHelper.FutureVersion();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();

        public override IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() => throw NotSupportedHelper.FutureVersion();

        public override MethodImplAttributes GetMethodImplementationFlags() => throw NotSupportedHelper.FutureVersion();

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw NotSupportedHelper.FutureVersion();

        public override bool IsDefined(Type attributeType, bool inherit) => throw NotSupportedHelper.FutureVersion();
    }
}
