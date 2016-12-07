using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class Metadata : CodeElement, IDisposable
    {
        private readonly Lazy<IEnumerable<AssemblyFile>> _assemblyFiles;
        private readonly Lazy<IEnumerable<AssemblyReference>> _assemblyReferences;
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<IEnumerable<CustomDebugInformation>> _customDebugInformation;
        private readonly Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<IEnumerable<Document>> _documents;
        private readonly Lazy<IEnumerable<EventDefinition>> _eventDefinitions;
        private readonly Lazy<IEnumerable<ExportedType>> _exportedTypes;
        private readonly Lazy<IEnumerable<FieldDefinition>> _fieldDefinitions;
        private readonly Lazy<IEnumerable<ImportScope>> _importScopes;
        private readonly Lazy<IEnumerable<LocalConstant>> _localConstants;
        private readonly Lazy<IEnumerable<LocalScope>> _localScopes;
        private readonly Lazy<IEnumerable<LocalVariable>> _localVariables;
        private readonly Lazy<IEnumerable<ManifestResource>> _manifestResources;
        private readonly Lazy<IEnumerable<MemberReference>> _memberReferences;
        private readonly Lazy<IEnumerable<MethodDebugInformation>> _methodDebugInformation;
        private readonly Lazy<IEnumerable<MethodDefinitionBase>> _methodDefinitions;
        private readonly Lazy<ModuleDefinition> _moduleDefinition;
        private readonly Lazy<IEnumerable<PropertyDefinition>> _propertyDefinitions;
        private readonly Lazy<IEnumerable<TypeDefinition>> _typeDefinitions;
        private readonly Lazy<IEnumerable<TypeReference>> _typeReferences;

        public Metadata(FileInfo assemblyFile) : this(false, assemblyFile)
        {
        }

        public Metadata(FileInfo assemblyFile, FileInfo pdbFile) : this(false, assemblyFile, pdbFile)
        {
        }

        public Metadata(bool prefetchMetadata, FileInfo assemblyFile, FileInfo pdbFile = null) : base(new MetadataState(prefetchMetadata, assemblyFile, pdbFile))
        {
            if (MetadataState.HasMetadata)
            {
                HasMetadata = true;
                if (Reader.IsAssembly)
                {
                    AssemblyDefinition = GetCodeElement<AssemblyDefinition>(Handle.AssemblyDefinition);
                }
                _assemblyFiles = new Lazy<IEnumerable<AssemblyFile>>(() => GetCodeElements<AssemblyFile>(Reader.AssemblyFiles));
                _assemblyReferences = new Lazy<IEnumerable<AssemblyReference>>(() => GetCodeElements<AssemblyReference>(Reader.AssemblyReferences));
                _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(Reader.CustomAttributes));
                _declarativeSecurityAttributes = new Lazy<IEnumerable<DeclarativeSecurityAttribute>>(() => GetCodeElements<DeclarativeSecurityAttribute>(Reader.DeclarativeSecurityAttributes));
                _eventDefinitions = new Lazy<IEnumerable<EventDefinition>>(() => GetCodeElements<EventDefinition>(Reader.EventDefinitions));
                _exportedTypes = new Lazy<IEnumerable<ExportedType>>(() => GetCodeElements<ExportedType>(Reader.ExportedTypes));
                _fieldDefinitions = new Lazy<IEnumerable<FieldDefinition>>(() => GetCodeElements<FieldDefinition>(Reader.FieldDefinitions));
                _importScopes = new Lazy<IEnumerable<ImportScope>>(() => GetCodeElements<ImportScope>(Reader.ImportScopes));
                _manifestResources = new Lazy<IEnumerable<ManifestResource>>(() => GetCodeElements<ManifestResource>(Reader.ManifestResources));
                _memberReferences = new Lazy<IEnumerable<MemberReference>>(() => GetCodeElements<MemberReference>(Reader.MemberReferences));
                _methodDefinitions = new Lazy<IEnumerable<MethodDefinitionBase>>(() => GetCodeElements<MethodDefinitionBase>(Reader.MethodDefinitions));
                _moduleDefinition = new Lazy<ModuleDefinition>(() => GetCodeElement<ModuleDefinition>(Handle.ModuleDefinition));
                _propertyDefinitions = new Lazy<IEnumerable<PropertyDefinition>>(() => GetCodeElements<PropertyDefinition>(Reader.PropertyDefinitions));
                _typeDefinitions = new Lazy<IEnumerable<TypeDefinition>>(() => GetCodeElements<TypeDefinition>(Reader.TypeDefinitions));
                _typeReferences = new Lazy<IEnumerable<TypeReference>>(() => GetCodeElements<TypeReference>(Reader.TypeReferences));
            }
            if (MetadataState.HasDebugMetadata)
            {
                HasDebugMetadata = true;
                _customDebugInformation = new Lazy<IEnumerable<CustomDebugInformation>>(() => GetCodeElements<CustomDebugInformation>(MetadataState.PdbReader.CustomDebugInformation));
                _documents = new Lazy<IEnumerable<Document>>(() => GetCodeElements<Document>(MetadataState.PdbReader.Documents));
                _localConstants = new Lazy<IEnumerable<LocalConstant>>(() => GetCodeElements<LocalConstant>(MetadataState.PdbReader.LocalConstants));
                _localScopes = new Lazy<IEnumerable<LocalScope>>(() => GetCodeElements<LocalScope>(MetadataState.PdbReader.LocalScopes));
                _localVariables = new Lazy<IEnumerable<LocalVariable>>(() => GetCodeElements<LocalVariable>(MetadataState.PdbReader.LocalVariables));
                _methodDebugInformation = new Lazy<IEnumerable<MethodDebugInformation>>(() => GetCodeElements<MethodDebugInformation>(MetadataState.PdbReader.MethodDebugInformation));
            }
        }

        /*
        * MethodSpecificationHandle
        * CustomDebugInformationHandle
        */

        public AssemblyDefinition AssemblyDefinition { get; }

        public IEnumerable<AssemblyFile> AssemblyFiles => !HasMetadata ? null : _assemblyFiles.Value;

        public IEnumerable<AssemblyReference> AssemblyReferences => !HasMetadata ? null : _assemblyReferences.Value;

        public IEnumerable<CustomAttribute> CustomAttributes => !HasMetadata ? null : _customAttributes.Value;

        public IEnumerable<CustomDebugInformation> CustomDebugInformation => !HasDebugMetadata ? null : _customDebugInformation.Value;

        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => !HasMetadata ? null : _declarativeSecurityAttributes.Value;

        public IEnumerable<Document> Documents => !HasDebugMetadata ? null : _documents.Value;

        public IEnumerable<EventDefinition> EventDefinitions => !HasMetadata ? null : _eventDefinitions.Value;

        public IEnumerable<ExportedType> ExportedTypes => !HasMetadata ? null : _exportedTypes.Value;

        public IEnumerable<FieldDefinition> FieldDefinitions => !HasMetadata ? null : _fieldDefinitions.Value;

        public bool HasDebugMetadata { get; }

        public bool HasMetadata { get; }

        public IEnumerable<ImportScope> ImportScopes => !HasMetadata ? null : _importScopes.Value;

        public IEnumerable<LocalConstant> LocalConstants => !HasDebugMetadata ? null : _localConstants.Value;

        public IEnumerable<LocalScope> LocalScopes => !HasDebugMetadata ? null : _localScopes.Value;

        public IEnumerable<LocalVariable> LocalVariables => !HasDebugMetadata ? null : _localVariables.Value;

        public IEnumerable<ManifestResource> ManifestResources => !HasMetadata ? null : _manifestResources.Value;

        public IEnumerable<MemberReference> MemberReferences => !HasMetadata ? null : _memberReferences.Value;

        public IEnumerable<MethodDebugInformation> MethodDebugInformation => !HasDebugMetadata ? null : _methodDebugInformation.Value;

        public IEnumerable<MethodDefinitionBase> MethodDefinitions => !HasMetadata ? null : _methodDefinitions.Value;

        public ModuleDefinition ModuleDefinition => !HasMetadata ? null : _moduleDefinition.Value;

        public IEnumerable<PropertyDefinition> PropertyDefinitions => !HasMetadata ? null : _propertyDefinitions.Value;

        public IEnumerable<TypeDefinition> TypeDefinitions => !HasMetadata ? null : _typeDefinitions.Value;

        public IEnumerable<TypeReference> TypeReferences => !HasMetadata ? null : _typeReferences.Value;

        protected override sealed MetadataReader Reader => MetadataState.AssemblyReader ?? MetadataState.PdbReader;

        public void Dispose() => MetadataState.Dispose();
    }
}
