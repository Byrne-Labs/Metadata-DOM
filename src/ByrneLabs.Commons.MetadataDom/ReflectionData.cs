using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class ReflectionData : CodeElement, IDisposable
    {
        private readonly Lazy<IEnumerable<AssemblyFile>> _assemblyFiles;
        private readonly Lazy<IEnumerable<AssemblyReference>> _assemblyReferences;
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<IEnumerable<CustomDebugInformation>> _customDebugInformation;
        private readonly Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<IEnumerable<Document>> _documents;
        private readonly Lazy<IEnumerable<EventDefinition>> _eventDefinitions;
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

        public ReflectionData(FileInfo assemblyFile) : this(false, assemblyFile)
        {
        }

        public ReflectionData(FileInfo assemblyFile, FileInfo pdbFile) : this(false, assemblyFile, pdbFile)
        {
        }

        public ReflectionData(bool prefetchMetadata, FileInfo assemblyFile, FileInfo pdbFile = null) : base(assemblyFile ?? pdbFile, new MetadataState(prefetchMetadata, assemblyFile, pdbFile))
        {
            if (MetadataState.HasMetadata)
            {
                HasMetadata = true;
                if (Reader.IsAssembly)
                {
                    AssemblyDefinition = GetCodeElementWithHandle<AssemblyDefinition>(Handle.AssemblyDefinition);
                }
                _assemblyFiles = GetLazyCodeElementsWithHandle<AssemblyFile>(Reader.AssemblyFiles);
                _assemblyReferences = GetLazyCodeElementsWithHandle<AssemblyReference>(Reader.AssemblyReferences);
                _customAttributes = GetLazyCodeElementsWithHandle<CustomAttribute>(Reader.CustomAttributes);
                _declarativeSecurityAttributes = GetLazyCodeElementsWithHandle<DeclarativeSecurityAttribute>(Reader.DeclarativeSecurityAttributes);
                _eventDefinitions = GetLazyCodeElementsWithHandle<EventDefinition>(Reader.EventDefinitions);
                _fieldDefinitions = GetLazyCodeElementsWithHandle<FieldDefinition>(Reader.FieldDefinitions);
                _importScopes = GetLazyCodeElementsWithHandle<ImportScope>(Reader.ImportScopes);
                _manifestResources = GetLazyCodeElementsWithHandle<ManifestResource>(Reader.ManifestResources);
                _memberReferences = GetLazyCodeElementsWithHandle<MemberReference>(Reader.MemberReferences);
                _methodDefinitions = GetLazyCodeElementsWithHandle<MethodDefinitionBase>(Reader.MethodDefinitions);
                _moduleDefinition = GetLazyCodeElementWithHandle<ModuleDefinition>(Handle.ModuleDefinition);
                _propertyDefinitions = GetLazyCodeElementsWithHandle<PropertyDefinition>(Reader.PropertyDefinitions);
                _typeDefinitions = GetLazyCodeElementsWithHandle<TypeDefinition>(Reader.TypeDefinitions);
                _typeReferences = GetLazyCodeElementsWithHandle<TypeReference>(Reader.TypeReferences);
                MetadataKind = Reader.MetadataKind;
            }
            if (MetadataState.HasDebugMetadata)
            {
                HasDebugMetadata = true;
                _customDebugInformation = GetLazyCodeElementsWithHandle<CustomDebugInformation>(MetadataState.PdbReader.CustomDebugInformation);
                _documents = GetLazyCodeElementsWithHandle<Document>(MetadataState.PdbReader.Documents);
                _localConstants = GetLazyCodeElementsWithHandle<LocalConstant>(MetadataState.PdbReader.LocalConstants);
                _localScopes = GetLazyCodeElementsWithHandle<LocalScope>(MetadataState.PdbReader.LocalScopes);
                _localVariables = GetLazyCodeElementsWithHandle<LocalVariable>(MetadataState.PdbReader.LocalVariables);
                _methodDebugInformation = GetLazyCodeElementsWithHandle<MethodDebugInformation>(MetadataState.PdbReader.MethodDebugInformation);
            }
        }

        /// <inheritdoc cref="MetadataReader.GetAssemblyDefinition" />
        public AssemblyDefinition AssemblyDefinition { get; }

        /// <inheritdoc cref="MetadataReader.AssemblyFiles" />
        public IEnumerable<AssemblyFile> AssemblyFiles => !HasMetadata ? null : _assemblyFiles.Value;

        /// <inheritdoc cref="MetadataReader.AssemblyReferences" />
        public IEnumerable<AssemblyReference> AssemblyReferences => !HasMetadata ? null : _assemblyReferences.Value;

        /// <inheritdoc cref="MetadataReader.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => !HasMetadata ? null : _customAttributes.Value;

        /// <inheritdoc cref="MetadataReader.CustomDebugInformation" />
        public IEnumerable<CustomDebugInformation> CustomDebugInformation => !HasDebugMetadata ? null : _customDebugInformation.Value;

        /// <inheritdoc cref="MetadataReader.DeclarativeSecurityAttributes" />
        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => !HasMetadata ? null : _declarativeSecurityAttributes.Value;

        /// <inheritdoc cref="MetadataReader.Documents" />
        public IEnumerable<Document> Documents => !HasDebugMetadata ? null : _documents.Value;

        /// <inheritdoc cref="MetadataReader.EventDefinitions" />
        public IEnumerable<EventDefinition> EventDefinitions => !HasMetadata ? null : _eventDefinitions.Value;

        /// <inheritdoc cref="MetadataReader.ExportedTypes" />
        public IEnumerable<ExportedType> ExportedTypes => MetadataState.ExportedTypes;

        /// <inheritdoc cref="MetadataReader.FieldDefinitions" />
        public IEnumerable<FieldDefinition> FieldDefinitions => !HasMetadata ? null : _fieldDefinitions.Value;

        /// <summary>False if no PDB file found or if data could not be decoded; else true</summary>
        public bool HasDebugMetadata { get; }

        /// <inheritdoc cref="PEReader.HasMetadata" />
        public bool HasMetadata { get; }

        /// <inheritdoc cref="MetadataReader.ImportScopes" />
        public IEnumerable<ImportScope> ImportScopes => !HasMetadata ? null : _importScopes.Value;

        /// <inheritdoc cref="MetadataReader.LocalConstants" />
        public IEnumerable<LocalConstant> LocalConstants => !HasDebugMetadata ? null : _localConstants.Value;

        /// <inheritdoc cref="MetadataReader.LocalScopes" />
        public IEnumerable<LocalScope> LocalScopes => !HasDebugMetadata ? null : _localScopes.Value;

        /// <inheritdoc cref="MetadataReader.LocalVariables" />
        public IEnumerable<LocalVariable> LocalVariables => !HasDebugMetadata ? null : _localVariables.Value;

        /// <inheritdoc cref="MetadataReader.ManifestResources" />
        public IEnumerable<ManifestResource> ManifestResources => !HasMetadata ? null : _manifestResources.Value;

        /// <inheritdoc cref="MetadataReader.MemberReferences" />
        public IEnumerable<MemberReference> MemberReferences => !HasMetadata ? null : _memberReferences.Value;

        /// <inheritdoc cref="MetadataReader.MetadataKind" />
        public MetadataKind MetadataKind { get; }

        /// <inheritdoc cref="MetadataReader.MethodDebugInformation" />
        public IEnumerable<MethodDebugInformation> MethodDebugInformation => !HasDebugMetadata ? null : _methodDebugInformation.Value;

        /// <inheritdoc cref="MetadataReader.MethodDefinitions" />
        public IEnumerable<MethodDefinitionBase> MethodDefinitions => !HasMetadata ? null : _methodDefinitions.Value;

        /// <inheritdoc cref="MetadataReader.GetModuleDefinition" />
        public ModuleDefinition ModuleDefinition => !HasMetadata ? null : _moduleDefinition.Value;

        /// <inheritdoc cref="MetadataReader.PropertyDefinitions" />
        public IEnumerable<PropertyDefinition> PropertyDefinitions => !HasMetadata ? null : _propertyDefinitions.Value;

        /// <inheritdoc cref="MetadataReader.TypeDefinitions" />
        public IEnumerable<TypeDefinition> TypeDefinitions => !HasMetadata ? null : _typeDefinitions.Value;

        /// <inheritdoc cref="MetadataReader.TypeReferences" />
        public IEnumerable<TypeReference> TypeReferences => !HasMetadata ? null : _typeReferences.Value;

        protected override sealed MetadataReader Reader => MetadataState.AssemblyReader ?? MetadataState.PdbReader;

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                MetadataState.Dispose();
            }
        }
    }
}
