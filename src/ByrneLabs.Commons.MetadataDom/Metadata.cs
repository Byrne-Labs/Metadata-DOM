using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class Metadata : CodeElement, IDisposable
    {
        private readonly Lazy<IReadOnlyList<AssemblyFile>> _assemblyFiles;
        private readonly Lazy<IReadOnlyList<AssemblyReference>> _assemblyReferences;
        private readonly Lazy<IReadOnlyList<CustomAttribute>> _customAttributes;
        private readonly Lazy<IReadOnlyList<CustomDebugInformation>> _customDebugInformation;
        private readonly Lazy<IReadOnlyList<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<IReadOnlyList<Document>> _documents;
        private readonly Lazy<IReadOnlyList<EventDefinition>> _eventDefinitions;
        private readonly Lazy<IReadOnlyList<ExportedType>> _exportedTypes;
        private readonly Lazy<IReadOnlyList<FieldDefinition>> _fieldDefinitions;
        private readonly Lazy<IReadOnlyList<ImportScope>> _importScopes;
        private readonly Lazy<IReadOnlyList<LocalConstant>> _localConstants;
        private readonly Lazy<IReadOnlyList<LocalScope>> _localScopes;
        private readonly Lazy<IReadOnlyList<LocalVariable>> _localVariables;
        private readonly Lazy<IReadOnlyList<ManifestResource>> _manifestResources;
        private readonly Lazy<IReadOnlyList<MemberReference>> _memberReferences;
        private readonly Lazy<IReadOnlyList<MethodDebugInformation>> _methodDebugInformation;
        private readonly Lazy<IReadOnlyList<MethodDefinition>> _methodDefinitions;
        private readonly Lazy<ModuleDefinition> _moduleDefinition;
        private readonly Lazy<IReadOnlyList<PropertyDefinition>> _propertyDefinitions;
        private readonly Lazy<IReadOnlyList<TypeDefinition>> _typeDefinitions;
        private readonly Lazy<IReadOnlyList<TypeReference>> _typeReferences;

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
                _assemblyFiles = new Lazy<IReadOnlyList<AssemblyFile>>(() => GetCodeElements<AssemblyFile>(Reader.AssemblyFiles));
                _assemblyReferences = new Lazy<IReadOnlyList<AssemblyReference>>(() => GetCodeElements<AssemblyReference>(Reader.AssemblyReferences));
                _customAttributes = new Lazy<IReadOnlyList<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(Reader.CustomAttributes));
                _declarativeSecurityAttributes = new Lazy<IReadOnlyList<DeclarativeSecurityAttribute>>(() => GetCodeElements<DeclarativeSecurityAttribute>(Reader.DeclarativeSecurityAttributes));
                _eventDefinitions = new Lazy<IReadOnlyList<EventDefinition>>(() => GetCodeElements<EventDefinition>(Reader.EventDefinitions));
                _exportedTypes = new Lazy<IReadOnlyList<ExportedType>>(() => GetCodeElements<ExportedType>(Reader.ExportedTypes));
                _fieldDefinitions = new Lazy<IReadOnlyList<FieldDefinition>>(() => GetCodeElements<FieldDefinition>(Reader.FieldDefinitions));
                _importScopes = new Lazy<IReadOnlyList<ImportScope>>(() => GetCodeElements<ImportScope>(Reader.ImportScopes));
                _manifestResources = new Lazy<IReadOnlyList<ManifestResource>>(() => GetCodeElements<ManifestResource>(Reader.ManifestResources));
                _memberReferences = new Lazy<IReadOnlyList<MemberReference>>(() => GetCodeElements<MemberReference>(Reader.MemberReferences));
                _methodDefinitions = new Lazy<IReadOnlyList<MethodDefinition>>(() => GetCodeElements<MethodDefinition>(Reader.MethodDefinitions));
                _moduleDefinition = new Lazy<ModuleDefinition>(() => GetCodeElement<ModuleDefinition>(Handle.ModuleDefinition));
                _propertyDefinitions = new Lazy<IReadOnlyList<PropertyDefinition>>(() => GetCodeElements<PropertyDefinition>(Reader.PropertyDefinitions));
                _typeDefinitions = new Lazy<IReadOnlyList<TypeDefinition>>(() => GetCodeElements<TypeDefinition>(Reader.TypeDefinitions));
                _typeReferences = new Lazy<IReadOnlyList<TypeReference>>(() => GetCodeElements<TypeReference>(Reader.TypeReferences));
            }
            if (MetadataState.HasDebugMetadata)
            {
                HasDebugMetadata = true;
                _customDebugInformation = new Lazy<IReadOnlyList<CustomDebugInformation>>(() => GetCodeElements<CustomDebugInformation>(MetadataState.PdbReader.CustomDebugInformation));
                _documents = new Lazy<IReadOnlyList<Document>>(() => GetCodeElements<Document>(MetadataState.PdbReader.Documents));
                _localConstants = new Lazy<IReadOnlyList<LocalConstant>>(() => GetCodeElements<LocalConstant>(MetadataState.PdbReader.LocalConstants));
                _localScopes = new Lazy<IReadOnlyList<LocalScope>>(() => GetCodeElements<LocalScope>(MetadataState.PdbReader.LocalScopes));
                _localVariables = new Lazy<IReadOnlyList<LocalVariable>>(() => GetCodeElements<LocalVariable>(MetadataState.PdbReader.LocalVariables));
                _methodDebugInformation = new Lazy<IReadOnlyList<MethodDebugInformation>>(() => GetCodeElements<MethodDebugInformation>(MetadataState.PdbReader.MethodDebugInformation));
            }
        }

        /*
        * MethodSpecificationHandle
        * CustomDebugInformationHandle
        */

        public AssemblyDefinition AssemblyDefinition { get; }

        public IReadOnlyList<AssemblyFile> AssemblyFiles => !HasMetadata ? null : _assemblyFiles.Value;

        public IReadOnlyList<AssemblyReference> AssemblyReferences => !HasMetadata ? null : _assemblyReferences.Value;

        public IReadOnlyList<CustomAttribute> CustomAttributes => !HasMetadata ? null : _customAttributes.Value;

        public IReadOnlyList<CustomDebugInformation> CustomDebugInformation => !HasDebugMetadata ? null : _customDebugInformation.Value;

        public IReadOnlyList<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => !HasMetadata ? null : _declarativeSecurityAttributes.Value;

        public IReadOnlyList<Document> Documents => !HasDebugMetadata ? null : _documents.Value;

        public IReadOnlyList<EventDefinition> EventDefinitions => !HasMetadata ? null : _eventDefinitions.Value;

        public IReadOnlyList<ExportedType> ExportedTypes => !HasMetadata ? null : _exportedTypes.Value;

        public IReadOnlyList<FieldDefinition> FieldDefinitions => !HasMetadata ? null : _fieldDefinitions.Value;

        public bool HasDebugMetadata { get; }

        public bool HasMetadata { get; }

        public IReadOnlyList<ImportScope> ImportScopes => !HasMetadata ? null : _importScopes.Value;

        public IReadOnlyList<LocalConstant> LocalConstants => !HasDebugMetadata ? null : _localConstants.Value;

        public IReadOnlyList<LocalScope> LocalScopes => !HasDebugMetadata ? null : _localScopes.Value;

        public IReadOnlyList<LocalVariable> LocalVariables => !HasDebugMetadata ? null : _localVariables.Value;

        public IReadOnlyList<ManifestResource> ManifestResources => !HasMetadata ? null : _manifestResources.Value;

        public IReadOnlyList<MemberReference> MemberReferences => !HasMetadata ? null : _memberReferences.Value;

        public IReadOnlyList<MethodDebugInformation> MethodDebugInformation => !HasDebugMetadata ? null : _methodDebugInformation.Value;

        public IReadOnlyList<MethodDefinition> MethodDefinitions => !HasMetadata ? null : _methodDefinitions.Value;

        public ModuleDefinition ModuleDefinition => !HasMetadata ? null : _moduleDefinition.Value;

        public IReadOnlyList<PropertyDefinition> PropertyDefinitions => !HasMetadata ? null : _propertyDefinitions.Value;

        public IReadOnlyList<TypeDefinition> TypeDefinitions => !HasMetadata ? null : _typeDefinitions.Value;

        public IReadOnlyList<TypeReference> TypeReferences => !HasMetadata ? null : _typeReferences.Value;

        protected override sealed MetadataReader Reader => MetadataState.AssemblyReader ?? MetadataState.PdbReader;

        public void Dispose() => MetadataState.Dispose();
    }
}
