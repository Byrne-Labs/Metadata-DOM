using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ExportedType" />
    [PublicAPI]
    public class ExportedType : RuntimeCodeElement, ICodeElementWithHandle<ExportedTypeHandle, System.Reflection.Metadata.ExportedType>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _implementation;
        private readonly Lazy<NamespaceDefinition> _namespaceDefinition;

        internal ExportedType(ExportedTypeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetExportedType(metadataHandle);
            Name = AsString(MetadataToken.Name);
            Attributes = MetadataToken.Attributes;
            _implementation = GetLazyCodeElementWithHandle(MetadataToken.Implementation);
            IsForwarder = MetadataToken.IsForwarder;
            Namespace = AsString(MetadataToken.Namespace);
            _namespaceDefinition = GetLazyCodeElementWithHandle<NamespaceDefinition>(MetadataToken.NamespaceDefinition);
            _customAttributes = GetLazyCodeElementsWithHandle<CustomAttribute>(MetadataToken.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.Attributes" />
        public TypeAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

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

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.Namespace" />
        public string Namespace { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.NamespaceDefinition" />
        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public ExportedTypeHandle MetadataHandle { get; }

        public System.Reflection.Metadata.ExportedType MetadataToken { get; }
    }
}
