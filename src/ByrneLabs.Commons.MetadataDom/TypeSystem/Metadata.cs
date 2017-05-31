using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class Metadata : IManagedCodeElement, IDisposable
    {
        private readonly Lazy<ImmutableArray<AssemblyFile>> _assemblyFiles;
        private readonly Lazy<ImmutableArray<CustomDebugInformation>> _customDebugInformation;
        private readonly Lazy<ImmutableArray<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<ImmutableArray<Document>> _documents;
        private readonly Lazy<ImmutableArray<EventDefinition>> _eventDefinitions;
        private readonly Lazy<ImmutableArray<ExportedType>> _exportedTypes;
        private readonly Lazy<ImmutableArray<FieldDefinition>> _fieldDefinitions;
        private readonly Lazy<ImmutableArray<ImportScope>> _importScopes;
        private readonly Lazy<ImmutableArray<Language>> _languages;
        private readonly Lazy<ImmutableArray<LocalConstant>> _localConstants;
        private readonly Lazy<ImmutableArray<LocalScope>> _localScopes;
        private readonly Lazy<ImmutableArray<LocalVariable>> _localVariables;
        private readonly Lazy<ImmutableArray<ManifestResource>> _manifestResources;
        private readonly Lazy<ImmutableArray<IMemberInfo>> _memberDefinitions;
        private readonly Lazy<ImmutableArray<MethodBase>> _methodDefinitions;
        private readonly Lazy<ImmutableArray<PropertyDefinition>> _propertyDefinitions;
        private readonly Lazy<ImmutableArray<TypeDefinition>> _typeDefinitions;
        private readonly Lazy<ImmutableArray<TypeReference>> _typeReferences;

        public Metadata(FileInfo assemblyFile) : this(false, assemblyFile)
        {
        }

        public Metadata(FileInfo assemblyFile, FileInfo pdbFile) : this(false, assemblyFile, pdbFile)
        {
        }

        public Metadata(bool prefetchMetadata, FileInfo assemblyFile, FileInfo pdbFile = null)
        {
            Key = new CodeElementKey<Metadata>(this);
            MetadataState = new MetadataState(prefetchMetadata, assemblyFile, pdbFile);
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
                _exportedTypes = MetadataState.GetLazyCodeElements<ExportedType>(Reader.ExportedTypes);
                _fieldDefinitions = MetadataState.GetLazyCodeElements<FieldDefinition>(Reader.FieldDefinitions);
                _importScopes = MetadataState.GetLazyCodeElements<ImportScope>(Reader.ImportScopes);
                _manifestResources = MetadataState.GetLazyCodeElements<ManifestResource>(Reader.ManifestResources);
                _methodDefinitions = new Lazy<ImmutableArray<MethodBase>>(() => MetadataState.GetCodeElements(Reader.MethodDefinitions).Cast<MethodBase>().ToImmutableArray());
                _propertyDefinitions = MetadataState.GetLazyCodeElements<PropertyDefinition>(Reader.PropertyDefinitions);
                _typeDefinitions = new Lazy<ImmutableArray<TypeDefinition>>(() => MetadataState.GetCodeElements<TypeDefinition>(Reader.TypeDefinitions).Where(typeDefinition => !"<Module>".Equals(typeDefinition.Name)).ToImmutableArray());
                _typeReferences = MetadataState.GetLazyCodeElements<TypeReference>(Reader.TypeReferences);
                _memberDefinitions = new Lazy<ImmutableArray<IMemberInfo>>(() => MethodDefinitions.Cast<IMemberInfo>().Union(FieldDefinitions).Union(EventDefinitions).Union(PropertyDefinitions).Union(TypeDefinitions).ToImmutableArray());
                MetadataKind = Reader.MetadataKind;
            }
            else
            {
                _assemblyFiles = new Lazy<ImmutableArray<AssemblyFile>>(() => ImmutableArray<AssemblyFile>.Empty);
                _declarativeSecurityAttributes = new Lazy<ImmutableArray<DeclarativeSecurityAttribute>>(() => ImmutableArray<DeclarativeSecurityAttribute>.Empty);
                _eventDefinitions = new Lazy<ImmutableArray<EventDefinition>>(() => ImmutableArray<EventDefinition>.Empty);
                _exportedTypes = new Lazy<ImmutableArray<ExportedType>>(() => ImmutableArray<ExportedType>.Empty);
                _fieldDefinitions = new Lazy<ImmutableArray<FieldDefinition>>(() => ImmutableArray<FieldDefinition>.Empty);
                _importScopes = new Lazy<ImmutableArray<ImportScope>>(() => ImmutableArray<ImportScope>.Empty);
                _manifestResources = new Lazy<ImmutableArray<ManifestResource>>(() => ImmutableArray<ManifestResource>.Empty);
                _methodDefinitions = new Lazy<ImmutableArray<MethodBase>>(() => ImmutableArray<MethodBase>.Empty);
                _propertyDefinitions = new Lazy<ImmutableArray<PropertyDefinition>>(() => ImmutableArray<PropertyDefinition>.Empty);
                _typeDefinitions = new Lazy<ImmutableArray<TypeDefinition>>(() => ImmutableArray<TypeDefinition>.Empty);
                _typeReferences = new Lazy<ImmutableArray<TypeReference>>(() => ImmutableArray<TypeReference>.Empty);
                _memberDefinitions = new Lazy<ImmutableArray<IMemberInfo>>(() => ImmutableArray<IMemberInfo>.Empty);
            }
            if (MetadataState.HasDebugMetadata)
            {
                HasDebugMetadata = true;
                _customDebugInformation = MetadataState.GetLazyCodeElements<CustomDebugInformation>(MetadataState.PdbReader.CustomDebugInformation);
                _documents = MetadataState.GetLazyCodeElements<Document>(MetadataState.PdbReader.Documents);
                _languages = new Lazy<ImmutableArray<Language>>(() => Documents.Select(document => document.Language).Distinct().ToImmutableArray());
                _localConstants = MetadataState.GetLazyCodeElements<LocalConstant>(MetadataState.PdbReader.LocalConstants);
                _localScopes = MetadataState.GetLazyCodeElements<LocalScope>(MetadataState.PdbReader.LocalScopes);
                _localVariables = MetadataState.GetLazyCodeElements<LocalVariable>(MetadataState.PdbReader.LocalVariables);
            }
            else
            {
                _customDebugInformation = new Lazy<ImmutableArray<CustomDebugInformation>>(() => ImmutableArray<CustomDebugInformation>.Empty);
                _documents = new Lazy<ImmutableArray<Document>>(() => ImmutableArray<Document>.Empty);
                _languages = new Lazy<ImmutableArray<Language>>(() => ImmutableArray<Language>.Empty);
                _localConstants = new Lazy<ImmutableArray<LocalConstant>>(() => ImmutableArray<LocalConstant>.Empty);
                _localScopes = new Lazy<ImmutableArray<LocalScope>>(() => ImmutableArray<LocalScope>.Empty);
                _localVariables = new Lazy<ImmutableArray<LocalVariable>>(() => ImmutableArray<LocalVariable>.Empty);
            }
        }

        public AssemblyDefinition AssemblyDefinition { get; }

        public FileInfo AssemblyFile { get; }

        public ImmutableArray<AssemblyFile> AssemblyFiles => _assemblyFiles.Value;

        public ImmutableArray<AssemblyReference> AssemblyReferences => MetadataState.AssemblyReferences;

        public ImmutableArray<CustomDebugInformation> CustomDebugInformation => _customDebugInformation.Value;

        public ImmutableArray<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public ImmutableArray<Document> Documents => _documents.Value;

        public ImmutableArray<EventDefinition> EventDefinitions => !HasMetadata ? ImmutableArray<EventDefinition>.Empty : _eventDefinitions.Value;

        public ImmutableArray<ExportedType> ExportedTypes => _exportedTypes.Value;

        public ImmutableArray<FieldDefinition> FieldDefinitions => _fieldDefinitions.Value;

        public bool HasDebugMetadata { get; }

        public bool HasMetadata { get; }

        public ImmutableArray<ImportScope> ImportScopes => _importScopes.Value;

        public ImmutableArray<Language> Languages => _languages.Value;

        public ImmutableArray<LocalConstant> LocalConstants => _localConstants.Value;

        public ImmutableArray<LocalScope> LocalScopes => _localScopes.Value;

        public ImmutableArray<LocalVariable> LocalVariables => _localVariables.Value;

        public ImmutableArray<ManifestResource> ManifestResources => _manifestResources.Value;

        public ImmutableArray<IMemberInfo> MemberDefinitions => _memberDefinitions.Value;

        public MetadataKind MetadataKind { get; }

        public ImmutableArray<MethodBase> MethodDefinitions => _methodDefinitions.Value;

        public ModuleDefinition ModuleDefinition => MetadataState.ModuleDefinition;

        public FileInfo PdbFile { get; }

        public ImmutableArray<PropertyDefinition> PropertyDefinitions => _propertyDefinitions.Value;

        public ImmutableArray<TypeDefinition> TypeDefinitions => _typeDefinitions.Value;

        public ImmutableArray<TypeReference> TypeReferences => _typeReferences.Value;

        protected MetadataReader Reader => MetadataState.AssemblyReader ?? MetadataState.PdbReader;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

        protected virtual void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                MetadataState.Dispose();
            }
        }
    }
}
