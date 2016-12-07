using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ExportedType" />
    public class ExportedType : CodeElementWithHandle
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<CodeElement> _implementation;
        private readonly Lazy<string> _name;
        private readonly Lazy<string> _namespace;
        private readonly Lazy<NamespaceDefinition> _namespaceDefinition;

        internal ExportedType(ExportedTypeHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var exportedType = Reader.GetExportedType(metadataHandle);
            _name = new Lazy<string>(() => AsString(exportedType.Name));
            Attributes = exportedType.Attributes;
            _implementation = new Lazy<CodeElement>(() => GetCodeElement(exportedType.Implementation));
            IsForwarder = exportedType.IsForwarder;
            _namespace = new Lazy<string>(() => AsString(exportedType.Namespace));
            _namespaceDefinition = new Lazy<NamespaceDefinition>(() => GetCodeElement<NamespaceDefinition>(exportedType.NamespaceDefinition));
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(exportedType.GetCustomAttributes()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.Attributes" />
        public TypeAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.Implementation" />
        /// <returns>
        ///     <list type="bullet">
        ///         <item><description><see cref="T:ByrneLabs.Commons.MetadataDom.AssemblyFile" /> representing another module in the assembly.</description></item>
        ///         <item>
        ///             <description><see cref="T:ByrneLabs.Commons.MetadataDom.AssemblyReference" /> representing another assembly if
        ///                 <see cref="P:ByrneLabs.Commons.MetadataDom.ExportedType.IsForwarder" /> is true.</description>
        ///         </item>
        ///         <item>
        ///             <description><see cref="T:ByrneLabs.Commons.MetadataDom.ExportedType" /> representing the declaring exported type in which this was is nested.</description>
        ///         </item>
        ///     </list>
        /// </returns>
        public CodeElement Implementation => _implementation.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.IsForwarder" />
        public bool IsForwarder { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.Name" />
        public string Name => _name.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.Namespace" />
        public string Namespace => _namespace.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ExportedType.NamespaceDefinition" />
        public NamespaceDefinition NamespaceDefinition => _namespaceDefinition.Value;
    }
}
