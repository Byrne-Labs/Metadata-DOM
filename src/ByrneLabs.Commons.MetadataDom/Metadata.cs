using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public class Metadata : CodeElement, IDisposable
    {
        private readonly Lazy<ImmutableArray<AssemblyFile>> _assemblyFiles;
        private readonly Lazy<ImmutableArray<CustomDebugInformation>> _customDebugInformation;
        private readonly Lazy<ImmutableArray<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<ImmutableArray<Document>> _documents;
        private readonly Lazy<ImmutableArray<EventDefinition>> _eventDefinitions;
        private readonly Lazy<ImmutableArray<FieldDefinition>> _fieldDefinitions;
        private readonly Lazy<ImmutableArray<ImportScope>> _importScopes;
        private readonly Lazy<ImmutableArray<Language>> _languages;
        private readonly Lazy<ImmutableArray<ManifestResource>> _manifestResources;
        private readonly Lazy<ImmutableArray<IMember>> _memberDefinitions;
        private readonly Lazy<ImmutableArray<MethodDefinitionBase>> _methodDefinitions;
        private readonly Lazy<ImmutableArray<PropertyDefinition>> _propertyDefinitions;
        private readonly Lazy<ImmutableArray<TypeDefinition>> _typeDefinitions;
        private readonly Lazy<ImmutableArray<TypeReference>> _typeReferences;

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
                _methodDefinitions = new Lazy<ImmutableArray<MethodDefinitionBase>>(() => MetadataState.GetCodeElements(Reader.MethodDefinitions).Cast<MethodDefinitionBase>().ToImmutableArray());
                _propertyDefinitions = MetadataState.GetLazyCodeElements<PropertyDefinition>(Reader.PropertyDefinitions);
                _typeDefinitions = new Lazy<ImmutableArray<TypeDefinition>>(() => MetadataState.GetCodeElements<TypeDefinition>(Reader.TypeDefinitions).Where(typeDefinition => !"<Module>".Equals(typeDefinition.Name)).ToImmutableArray());
                _typeReferences = MetadataState.GetLazyCodeElements<TypeReference>(Reader.TypeReferences);
                _memberDefinitions = new Lazy<ImmutableArray<IMember>>(() => MethodDefinitions.Union<IMember>(FieldDefinitions).Union(EventDefinitions).Union(PropertyDefinitions).Union(TypeDefinitions).ToImmutableArray());
                MetadataKind = Reader.MetadataKind;
            }
            else
            {
                _assemblyFiles = new Lazy<ImmutableArray<AssemblyFile>>(() => ImmutableArray<AssemblyFile>.Empty);
                _declarativeSecurityAttributes = new Lazy<ImmutableArray<DeclarativeSecurityAttribute>>(() => ImmutableArray<DeclarativeSecurityAttribute>.Empty);
                _eventDefinitions = new Lazy<ImmutableArray<EventDefinition>>(() => ImmutableArray<EventDefinition>.Empty);
                _fieldDefinitions = new Lazy<ImmutableArray<FieldDefinition>>(() => ImmutableArray<FieldDefinition>.Empty);
                _importScopes = new Lazy<ImmutableArray<ImportScope>>(() => ImmutableArray<ImportScope>.Empty);
                _manifestResources = new Lazy<ImmutableArray<ManifestResource>>(() => ImmutableArray<ManifestResource>.Empty);
                _methodDefinitions = new Lazy<ImmutableArray<MethodDefinitionBase>>(() => ImmutableArray<MethodDefinitionBase>.Empty);
                _propertyDefinitions = new Lazy<ImmutableArray<PropertyDefinition>>(() => ImmutableArray<PropertyDefinition>.Empty);
                _typeDefinitions = new Lazy<ImmutableArray<TypeDefinition>>(() => ImmutableArray<TypeDefinition>.Empty);
                _typeReferences = new Lazy<ImmutableArray<TypeReference>>(() => ImmutableArray<TypeReference>.Empty);
                _memberDefinitions = new Lazy<ImmutableArray<IMember>>(() => ImmutableArray<IMember>.Empty);
            }
            if (MetadataState.HasDebugMetadata)
            {
                HasDebugMetadata = true;
                _customDebugInformation = MetadataState.GetLazyCodeElements<CustomDebugInformation>(MetadataState.PdbReader.CustomDebugInformation);
                _documents = MetadataState.GetLazyCodeElements<Document>(MetadataState.PdbReader.Documents);
                _languages = new Lazy<ImmutableArray<Language>>(() => Documents.Select(document => document.Language).Distinct().ToImmutableArray());
            }
            else
            {
                _customDebugInformation = new Lazy<ImmutableArray<CustomDebugInformation>>(() => ImmutableArray<CustomDebugInformation>.Empty);
                _documents = new Lazy<ImmutableArray<Document>>(() => ImmutableArray<Document>.Empty);
                _languages = new Lazy<ImmutableArray<Language>>(() => ImmutableArray<Language>.Empty);
            }
        }

        /// <inheritdoc cref="MetadataReader.GetAssemblyDefinition" />
        public AssemblyDefinition AssemblyDefinition { get; }

        public FileInfo AssemblyFile { get; }

        /// <inheritdoc cref="MetadataReader.AssemblyFiles" />
        public ImmutableArray<AssemblyFile> AssemblyFiles => _assemblyFiles.Value;

        /// <inheritdoc cref="MetadataReader.AssemblyReferences" />
        public ImmutableArray<AssemblyReference> AssemblyReferences => MetadataState.AssemblyReferences;

        /// <inheritdoc cref="MetadataReader.CustomDebugInformation" />
        public ImmutableArray<CustomDebugInformation> CustomDebugInformation => _customDebugInformation.Value;

        /// <inheritdoc cref="MetadataReader.DeclarativeSecurityAttributes" />
        public ImmutableArray<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        /// <inheritdoc cref="MetadataReader.Documents" />
        public ImmutableArray<Document> Documents => _documents.Value;

        /// <inheritdoc cref="MetadataReader.EventDefinitions" />
        public ImmutableArray<EventDefinition> EventDefinitions => !HasMetadata ? ImmutableArray<EventDefinition>.Empty : _eventDefinitions.Value;

        public ImmutableArray<FieldDefinition> FieldDefinitions => _fieldDefinitions.Value;

        /// <summary>False if no PDB file found or if data could not be decoded; else true</summary>
        public bool HasDebugMetadata { get; }

        /// <inheritdoc cref="PEReader.HasMetadata" />
        public bool HasMetadata { get; }

        /// <inheritdoc cref="MetadataReader.ImportScopes" />
        public ImmutableArray<ImportScope> ImportScopes => _importScopes.Value;

        public ImmutableArray<Language> Languages => _languages.Value;

        /// <inheritdoc cref="MetadataReader.ManifestResources" />
        public ImmutableArray<ManifestResource> ManifestResources => _manifestResources.Value;

        public ImmutableArray<IMember> MemberDefinitions => _memberDefinitions.Value;

        /// <inheritdoc cref="MetadataReader.MetadataKind" />
        public MetadataKind MetadataKind { get; }

        /// <inheritdoc cref="MetadataReader.MethodDefinitions" />
        public ImmutableArray<MethodDefinitionBase> MethodDefinitions => _methodDefinitions.Value;

        /// <inheritdoc cref="MetadataReader.GetModuleDefinition" />
        public ModuleDefinition ModuleDefinition => MetadataState.ModuleDefinition;

        public FileInfo PdbFile { get; }

        /// <inheritdoc cref="MetadataReader.PropertyDefinitions" />
        public ImmutableArray<PropertyDefinition> PropertyDefinitions => _propertyDefinitions.Value;

        /// <inheritdoc cref="MetadataReader.TypeDefinitions" />
        public ImmutableArray<TypeDefinition> TypeDefinitions => _typeDefinitions.Value;

        /// <inheritdoc cref="MetadataReader.TypeReferences" />
        public ImmutableArray<TypeReference> TypeReferences => _typeReferences.Value;

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
