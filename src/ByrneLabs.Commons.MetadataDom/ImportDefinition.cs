using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.ImportDefinition" />
    public class ImportDefinition : DebugCodeElementWithoutHandle
    {
        private readonly Lazy<Blob> _alias;
        private readonly Lazy<AssemblyReference> _targetAssembly;
        private readonly Lazy<Blob> _targetNamespace;
        private readonly Lazy<CodeElement> _targetType;

        internal ImportDefinition(System.Reflection.Metadata.ImportDefinition importDefinition, MetadataState metadataState) : base(importDefinition, metadataState)
        {
            Kind = importDefinition.Kind;
            switch (importDefinition.Kind)
            {
                case ImportDefinitionKind.AliasAssemblyNamespace:
                    _alias = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.Alias)));
                    _targetAssembly = new Lazy<AssemblyReference>(() => GetCodeElement<AssemblyReference>(importDefinition.TargetAssembly));
                    _targetNamespace = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.TargetNamespace)));
                    break;
                case ImportDefinitionKind.AliasAssemblyReference:
                    _alias = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.Alias)));
                    _targetAssembly = new Lazy<AssemblyReference>(() => GetCodeElement<AssemblyReference>(importDefinition.TargetAssembly));
                    break;
                case ImportDefinitionKind.AliasNamespace:
                case ImportDefinitionKind.ImportXmlNamespace:
                    _alias = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.Alias)));
                    _targetNamespace = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.TargetNamespace)));
                    break;
                case ImportDefinitionKind.AliasType:
                    _alias = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.Alias)));
                    _targetType = new Lazy<CodeElement>(() => GetCodeElement(importDefinition.TargetType));
                    break;
                case ImportDefinitionKind.ImportAssemblyNamespace:
                    _targetAssembly = new Lazy<AssemblyReference>(() => GetCodeElement<AssemblyReference>(importDefinition.TargetAssembly));
                    _targetNamespace = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.TargetNamespace)));
                    break;
                case ImportDefinitionKind.ImportAssemblyReferenceAlias:
                    _alias = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.Alias)));
                    break;
                case ImportDefinitionKind.ImportNamespace:
                    _targetNamespace = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(importDefinition.TargetNamespace)));
                    break;
                case ImportDefinitionKind.ImportType:
                    _targetType = new Lazy<CodeElement>(() => GetCodeElement(importDefinition.TargetType));
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
        public CodeElement TargetType => _targetType?.Value;
    }
}
