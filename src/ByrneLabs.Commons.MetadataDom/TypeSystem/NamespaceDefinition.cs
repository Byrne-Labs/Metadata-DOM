using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public class NamespaceDefinition : Namespace, IManagedCodeElement
    {
        private readonly Lazy<ImmutableArray<ExportedType>> _exportedTypes;
        private readonly Lazy<ImmutableArray<NamespaceDefinition>> _namespaceDefinitions;
        private readonly Lazy<NamespaceDefinition> _parent;
        private readonly Lazy<ImmutableArray<TypeDefinition>> _typeDefinitions;

        internal NamespaceDefinition(NamespaceDefinitionHandle metadataHandle, MetadataState metadataState)
        {
            Key = new CodeElementKey<NamespaceDefinition>(metadataHandle);
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.AssemblyReader.GetNamespaceDefinition(metadataHandle);
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            _parent = MetadataState.GetLazyCodeElement<NamespaceDefinition>(RawMetadata.Parent);
            _namespaceDefinitions = MetadataState.GetLazyCodeElements<NamespaceDefinition>(RawMetadata.NamespaceDefinitions);
            _typeDefinitions = MetadataState.GetLazyCodeElements<TypeDefinition>(RawMetadata.TypeDefinitions);
            _exportedTypes = MetadataState.GetLazyCodeElements<ExportedType>(RawMetadata.ExportedTypes);
        }

        public override IEnumerable<Type> ExportedTypes => _exportedTypes.Value;

        public string FullName => Name;

        public NamespaceDefinitionHandle MetadataHandle { get; }

        public override string Name { get; }

        public override IEnumerable<Namespace> Namespaces => _namespaceDefinitions.Value;

        public override Namespace Parent => _parent.Value;

        public System.Reflection.Metadata.NamespaceDefinition RawMetadata { get; }

        public string TextSignature => Name;

        public override IEnumerable<Type> TypeDefinitions => _typeDefinitions.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
