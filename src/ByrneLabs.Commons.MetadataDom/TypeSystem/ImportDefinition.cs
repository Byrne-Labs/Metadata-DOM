using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class ImportDefinition : Import, IManagedCodeElement
    {
        private readonly Lazy<Blob> _alias;
        private readonly Lazy<AssemblyReference> _targetAssembly;
        private readonly Lazy<Blob> _targetNamespace;
        private readonly Lazy<TypeBase> _targetType;

        internal ImportDefinition(System.Reflection.Metadata.ImportDefinition importDefinition, MetadataState metadataState)
        {
            Key = new CodeElementKey<ImportDefinition>(importDefinition);
            MetadataState = metadataState;
            RawMetadata = importDefinition;
            switch (importDefinition.Kind)
            {
                case ImportDefinitionKind.AliasAssemblyNamespace:
                case ImportDefinitionKind.AliasAssemblyReference:
                case ImportDefinitionKind.ImportXmlNamespace:
                case ImportDefinitionKind.AliasType:
                case ImportDefinitionKind.ImportAssemblyReferenceAlias:
                    _alias = MetadataState.GetLazyCodeElement<Blob>(importDefinition.Alias, true);
                    break;
                default:
                    _alias = new Lazy<Blob>(() => throw new InvalidOperationException());
                    break;
            }
            switch (importDefinition.Kind)
            {
                case ImportDefinitionKind.AliasAssemblyNamespace:
                case ImportDefinitionKind.AliasAssemblyReference:
                case ImportDefinitionKind.ImportAssemblyNamespace:
                    _targetAssembly = MetadataState.GetLazyCodeElement<AssemblyReference>(importDefinition.TargetAssembly);
                    break;
                default:
                    _targetAssembly = new Lazy<AssemblyReference>(() => throw new InvalidOperationException());
                    break;
            }
            switch (importDefinition.Kind)
            {
                case ImportDefinitionKind.AliasAssemblyNamespace:
                case ImportDefinitionKind.ImportXmlNamespace:
                case ImportDefinitionKind.ImportAssemblyNamespace:
                case ImportDefinitionKind.ImportNamespace:
                    _targetNamespace = MetadataState.GetLazyCodeElement<Blob>(importDefinition.TargetNamespace, true);
                    break;
                default:
                    _targetNamespace = new Lazy<Blob>(() => throw new InvalidOperationException());
                    break;
            }
            switch (importDefinition.Kind)
            {
                case ImportDefinitionKind.AliasType:
                case ImportDefinitionKind.ImportType:
                    _targetType = MetadataState.GetLazyCodeElement<TypeBase>(importDefinition.TargetType);
                    break;
                default:
                    _targetType = new Lazy<TypeBase>(() => throw new InvalidOperationException());
                    break;
            }
        }

        public override string Alias { get; }

        public System.Reflection.Metadata.ImportDefinition RawMetadata { get; }

        public override AssemblyName TargetAssembly { get; }

        public override string TargetNamespace { get; }

        public override TypeInfo TargetType => _targetType?.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
