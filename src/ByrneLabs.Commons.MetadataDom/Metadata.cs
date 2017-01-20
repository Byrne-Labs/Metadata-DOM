using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public class Metadata : CodeElement, IDisposable
    {
        private readonly Lazy<IEnumerable<AssemblyFile>> _assemblyFiles;
        private readonly Lazy<IEnumerable<CustomDebugInformation>> _customDebugInformation;
        private readonly Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<IEnumerable<Document>> _documents;
        private readonly Lazy<IEnumerable<EventDefinition>> _eventDefinitions;
        private readonly Lazy<IEnumerable<FieldDefinition>> _fieldDefinitions;
        private readonly Lazy<IEnumerable<ImportScope>> _importScopes;
        private readonly Lazy<IEnumerable<Language>> _languages;
        private readonly Lazy<IEnumerable<ManifestResource>> _manifestResources;
        private readonly Lazy<IEnumerable<IMember>> _memberDefinitions;
        private readonly Lazy<IEnumerable<MethodDefinitionBase>> _methodDefinitions;
        private readonly Lazy<IEnumerable<PropertyDefinition>> _propertyDefinitions;
        private readonly Lazy<IEnumerable<TypeDefinition>> _typeDefinitions;
        private readonly Lazy<IEnumerable<TypeReference>> _typeReferences;

        public Metadata(FileInfo assemblyFile) : this(false, assemblyFile)
        {
        }

        public Metadata(FileInfo assemblyFile, FileInfo pdbFile) : this(false, assemblyFile, pdbFile)
        {
        }

        public Metadata(bool prefetchMetadata, FileInfo assemblyFile, FileInfo pdbFile = null) : base(new CodeElementKey<Metadata>(assemblyFile ?? pdbFile), new MetadataState(prefetchMetadata, assemblyFile, pdbFile))
        {
            AssemblyFile = assemblyFile;
            PdbFile = pdbFile;
            if (MetadataState.HasMetadata)
            {
                HasMetadata = true;
                if (Reader.IsAssembly)
                {
                    AssemblyDefinition = MetadataState.GetCodeElement<AssemblyDefinition>(Handle.AssemblyDefinition);
                }
                _assemblyFiles = MetadataState.GetLazyCodeElements<AssemblyFile>(Reader.AssemblyFiles);
                _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(Reader.DeclarativeSecurityAttributes);
                _eventDefinitions = MetadataState.GetLazyCodeElements<EventDefinition>(Reader.EventDefinitions);
                _fieldDefinitions = MetadataState.GetLazyCodeElements<FieldDefinition>(Reader.FieldDefinitions);
                _importScopes = MetadataState.GetLazyCodeElements<ImportScope>(Reader.ImportScopes);
                _manifestResources = MetadataState.GetLazyCodeElements<ManifestResource>(Reader.ManifestResources);
                _methodDefinitions = new Lazy<IEnumerable<MethodDefinitionBase>>(() => MetadataState.GetCodeElements(Reader.MethodDefinitions).Cast<MethodDefinitionBase>());
                _propertyDefinitions = MetadataState.GetLazyCodeElements<PropertyDefinition>(Reader.PropertyDefinitions);
                _typeDefinitions = new Lazy<IEnumerable<TypeDefinition>>(() => MetadataState.GetCodeElements<TypeDefinition>(Reader.TypeDefinitions).Where(typeDefinition => !"<Module>".Equals(typeDefinition.Name)).ToList());
                _typeReferences = MetadataState.GetLazyCodeElements<TypeReference>(Reader.TypeReferences);
                _memberDefinitions = new Lazy<IEnumerable<IMember>>(() => MethodDefinitions.Union<IMember>(FieldDefinitions).Union(EventDefinitions).Union(PropertyDefinitions).Union(TypeDefinitions).ToList());
                MetadataKind = Reader.MetadataKind;
            }
            if (MetadataState.HasDebugMetadata)
            {
                HasDebugMetadata = true;
                _customDebugInformation = MetadataState.GetLazyCodeElements<CustomDebugInformation>(MetadataState.PdbReader.CustomDebugInformation);
                _documents = MetadataState.GetLazyCodeElements<Document>(MetadataState.PdbReader.Documents);
                _languages = new Lazy<IEnumerable<Language>>(() => Documents.Select(document => document.Language).Distinct().ToList());
            }
        }

        /// <inheritdoc cref="MetadataReader.GetAssemblyDefinition" />
        public AssemblyDefinition AssemblyDefinition { get; }

        public FileInfo AssemblyFile { get; }

        /// <inheritdoc cref="MetadataReader.AssemblyFiles" />
        public IEnumerable<AssemblyFile> AssemblyFiles => !HasMetadata ? new List<AssemblyFile>() : _assemblyFiles.Value;

        /// <inheritdoc cref="MetadataReader.AssemblyReferences" />
        public IEnumerable<AssemblyReference> AssemblyReferences => !HasMetadata ? new List<AssemblyReference>() : MetadataState.AssemblyReferences;

        /// <inheritdoc cref="MetadataReader.CustomDebugInformation" />
        public IEnumerable<CustomDebugInformation> CustomDebugInformation => !HasDebugMetadata ? new List<CustomDebugInformation>() : _customDebugInformation.Value;

        /// <inheritdoc cref="MetadataReader.DeclarativeSecurityAttributes" />
        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => !HasMetadata ? new List<DeclarativeSecurityAttribute>() : _declarativeSecurityAttributes.Value;

        /// <inheritdoc cref="MetadataReader.Documents" />
        public IEnumerable<Document> Documents => !HasDebugMetadata ? new List<Document>() : _documents.Value;

        /// <inheritdoc cref="MetadataReader.EventDefinitions" />
        public IEnumerable<EventDefinition> EventDefinitions => !HasMetadata ? new List<EventDefinition>() : _eventDefinitions.Value;

        public IEnumerable<FieldDefinition> FieldDefinitions => !HasMetadata ? new List<FieldDefinition>() : _fieldDefinitions.Value;

        /// <summary>False if no PDB file found or if data could not be decoded; else true</summary>
        public bool HasDebugMetadata { get; }

        /// <inheritdoc cref="PEReader.HasMetadata" />
        public bool HasMetadata { get; }

        /// <inheritdoc cref="MetadataReader.ImportScopes" />
        public IEnumerable<ImportScope> ImportScopes => !HasMetadata ? new List<ImportScope>() : _importScopes.Value;

        public IEnumerable<Language> Languages => !HasDebugMetadata ? new List<Language>() : _languages.Value;

        /// <inheritdoc cref="MetadataReader.ManifestResources" />
        public IEnumerable<ManifestResource> ManifestResources => !HasMetadata ? new List<ManifestResource>() : _manifestResources.Value;

        public IEnumerable<IMember> MemberDefinitions => !HasMetadata ? new List<IMember>() : _memberDefinitions.Value;

        /// <inheritdoc cref="MetadataReader.MetadataKind" />
        public MetadataKind MetadataKind { get; }

        /// <inheritdoc cref="MetadataReader.MethodDefinitions" />
        public IEnumerable<MethodDefinitionBase> MethodDefinitions => !HasMetadata ? new List<MethodDefinitionBase>() : _methodDefinitions.Value;

        /// <inheritdoc cref="MetadataReader.GetModuleDefinition" />
        public ModuleDefinition ModuleDefinition => MetadataState.ModuleDefinition;

        public FileInfo PdbFile { get; }

        /// <inheritdoc cref="MetadataReader.PropertyDefinitions" />
        public IEnumerable<PropertyDefinition> PropertyDefinitions => !HasMetadata ? new List<PropertyDefinition>() : _propertyDefinitions.Value;

        /// <inheritdoc cref="MetadataReader.TypeDefinitions" />
        public IEnumerable<TypeDefinition> TypeDefinitions => !HasMetadata ? new List<TypeDefinition>() : _typeDefinitions.Value;

        /// <inheritdoc cref="MetadataReader.TypeReferences" />
        public IEnumerable<TypeReference> TypeReferences => !HasMetadata ? new List<TypeReference>() : _typeReferences.Value;

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
