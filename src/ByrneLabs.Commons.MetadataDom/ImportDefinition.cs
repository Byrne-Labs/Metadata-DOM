using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ImportDefinition" />
    //[PublicAPI]
    public class ImportDefinition : DebugCodeElement, ICodeElementWithToken<System.Reflection.Metadata.ImportDefinition>
    {
        private readonly Lazy<Blob> _alias;
        private readonly Lazy<AssemblyReference> _targetAssembly;
        private readonly Lazy<Blob> _targetNamespace;
        private readonly Lazy<TypeBase> _targetType;

        internal ImportDefinition(System.Reflection.Metadata.ImportDefinition importDefinition, MetadataState metadataState) : base(new CodeElementKey<ImportDefinition>(importDefinition), metadataState)
        {
            MetadataToken = importDefinition;
            Kind = importDefinition.Kind;
            switch (importDefinition.Kind)
            {
                case ImportDefinitionKind.AliasAssemblyNamespace:
                    _alias = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.Alias)));
                    _targetAssembly = MetadataState.GetLazyCodeElement<AssemblyReference>(importDefinition.TargetAssembly);
                    _targetNamespace = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.TargetNamespace)));
                    break;
                case ImportDefinitionKind.AliasAssemblyReference:
                    _alias = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.Alias)));
                    _targetAssembly = MetadataState.GetLazyCodeElement<AssemblyReference>(importDefinition.TargetAssembly);
                    break;
                case ImportDefinitionKind.AliasNamespace:
                case ImportDefinitionKind.ImportXmlNamespace:
                    _alias = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.Alias)));
                    _targetNamespace = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.TargetNamespace)));
                    break;
                case ImportDefinitionKind.AliasType:
                    _alias = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.Alias)));
                    _targetType = new Lazy<TypeBase>(() => (TypeBase) MetadataState.GetCodeElement(importDefinition.TargetType));
                    break;
                case ImportDefinitionKind.ImportAssemblyNamespace:
                    _targetAssembly = MetadataState.GetLazyCodeElement<AssemblyReference>(importDefinition.TargetAssembly);
                    _targetNamespace = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.TargetNamespace)));
                    break;
                case ImportDefinitionKind.ImportAssemblyReferenceAlias:
                    _alias = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.Alias)));
                    break;
                case ImportDefinitionKind.ImportNamespace:
                    _targetNamespace = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.TargetNamespace)));
                    break;
                case ImportDefinitionKind.ImportType:
                    _targetType = new Lazy<TypeBase>(() => (TypeBase) MetadataState.GetCodeElement(importDefinition.TargetType));
                    break;
                default:
                    throw new ArgumentException($"Unknown ImportDefinitionKind {importDefinition.Kind}");
            }
        }

        /// <inheritdoc cref="System.Reflection.Metadata.ImportDefinition.Alias" />
        public Blob Alias => _alias?.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ImportDefinition.Kind" />
        public ImportDefinitionKind Kind { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.ImportDefinition.TargetAssembly" />
        public AssemblyReference TargetAssembly => _targetAssembly?.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ImportDefinition.TargetNamespace" />
        public Blob TargetNamespace => _targetNamespace?.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.ImportDefinition.TargetType" />
        public TypeBase TargetType => _targetType?.Value;

        public System.Reflection.Metadata.ImportDefinition MetadataToken { get; }
    }
}
