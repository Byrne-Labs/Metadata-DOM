using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class ExportedType : TypeBase<ExportedType, ExportedTypeHandle, System.Reflection.Metadata.ExportedType>
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _implementation;
        private readonly Lazy<NamespaceDefinition> _namespaceDefinition;

        internal ExportedType(ExportedTypeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            Attributes = RawMetadata.Attributes;
            _implementation = MetadataState.GetLazyCodeElement(RawMetadata.Implementation);
            IsForwarder = RawMetadata.IsForwarder;
            _namespaceDefinition = MetadataState.GetLazyCodeElement<NamespaceDefinition>(RawMetadata.NamespaceDefinition);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        public override IAssembly Assembly => MetadataState.AssemblyDefinition;
        public TypeAttributes Attributes { get; }
        public override ImmutableArray<CustomAttributeData> CustomAttributes => _customAttributes.Value;

        public override TypeBase DeclaringType { get; } = null;
        public CodeElement Implementation => _implementation.Value;
        public bool IsForwarder { get; }

        public override bool IsGenericParameter { get; } = false;

        public override MemberTypes MemberType { get; } = MemberTypes.Custom;

        public override string Namespace => AsString(RawMetadata.Namespace);
        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;

        internal override string UndecoratedName => AsString(RawMetadata.Name);

        protected override string MetadataNamespace { get; } = null;
    }
}
