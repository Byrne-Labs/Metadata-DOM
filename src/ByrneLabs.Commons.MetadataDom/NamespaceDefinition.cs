﻿using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition" />
    [PublicAPI]
    public class NamespaceDefinition : RuntimeCodeElement, ICodeElementWithHandle<NamespaceDefinitionHandle, System.Reflection.Metadata.NamespaceDefinition>
    {
        private readonly Lazy<IEnumerable<ExportedType>> _exportedTypes;
        private readonly Lazy<IEnumerable<NamespaceDefinition>> _namespaceDefinitions;
        private readonly Lazy<NamespaceDefinition> _parent;
        private readonly Lazy<IEnumerable<TypeDefinition>> _typeDefinitions;

        internal NamespaceDefinition(NamespaceDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetNamespaceDefinition(metadataHandle);
            Name = AsString(MetadataToken.Name);
            _parent = GetLazyCodeElementWithHandle<NamespaceDefinition>(MetadataToken.Parent);
            _namespaceDefinitions = GetLazyCodeElementsWithHandle<NamespaceDefinition>(MetadataToken.NamespaceDefinitions);
            _typeDefinitions = GetLazyCodeElementsWithHandle<TypeDefinition>(MetadataToken.TypeDefinitions);
            _exportedTypes = GetLazyCodeElementsWithHandle<ExportedType>(MetadataToken.ExportedTypes);
        }

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.ExportedTypes" />
        public IEnumerable<ExportedType> ExportedTypes => _exportedTypes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.NamespaceDefinitions" />
        public IEnumerable<NamespaceDefinition> NamespaceDefinitions => _namespaceDefinitions.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.Parent" />
        public NamespaceDefinition Parent => _parent.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.NamespaceDefinition.TypeDefinitions" />
        public IEnumerable<TypeDefinition> TypeDefinitions => _typeDefinitions.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public NamespaceDefinitionHandle MetadataHandle { get; }

        public System.Reflection.Metadata.NamespaceDefinition MetadataToken { get; }
    }
}
