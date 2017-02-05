using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ExportedType" />
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

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.Attributes" />
        public TypeAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.GetCustomAttributes" />
        public override ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public override TypeBase DeclaringType { get; } = null;

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.Implementation" />
        /// <returns>
        ///     <list type="bullet">
        ///         <item><description><see cref="AssemblyFile" /> representing another module in the assembly.</description></item>
        ///         <item>
        ///             <description><see cref="AssemblyReference" /> representing another assembly if
        ///                 <see cref="ExportedType.IsForwarder" /> is true.</description>
        ///         </item>
        ///         <item>
        ///             <description><see cref="ExportedType" /> representing the declaring exported type in which this was is nested.</description>
        ///         </item>
        ///     </list>
        /// </returns>
        public CodeElement Implementation => _implementation.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.IsForwarder" />
        public bool IsForwarder { get; }

        public override bool IsGenericParameter { get; } = false;

        public override MemberTypes MemberType { get; } = MemberTypes.Custom;

        public override string Namespace => AsString(RawMetadata.Namespace);

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.NamespaceDefinition" />
        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;

        internal override string UndecoratedName => AsString(RawMetadata.Name);

        protected override string MetadataNamespace { get; } = null;
    }
}
