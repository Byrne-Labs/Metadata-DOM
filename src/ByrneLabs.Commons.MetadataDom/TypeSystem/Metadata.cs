using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    public class Metadata : IManagedCodeElement, IDisposable
    {
        private readonly Lazy<IEnumerable<AssemblyFile>> _assemblyFiles;
        private readonly Lazy<IEnumerable<ConstructorDefinition>> _constructorDefinitions;
        private readonly Lazy<IEnumerable<CustomDebugInformation>> _customDebugInformation;
        private readonly Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<IEnumerable<Document>> _documents;
        private readonly Lazy<IEnumerable<EventDefinition>> _eventDefinitions;
        private readonly Lazy<IEnumerable<ExportedType>> _exportedTypes;
        private readonly Lazy<IEnumerable<FieldDefinition>> _fieldDefinitions;
        private readonly Lazy<IEnumerable<ImportScope>> _importScopes;
        private readonly Lazy<IEnumerable<Language>> _languages;
        private readonly Lazy<IEnumerable<LocalConstant>> _localConstants;
        private readonly Lazy<IEnumerable<LocalScope>> _localScopes;
        private readonly Lazy<IEnumerable<LocalVariable>> _localVariables;
        private readonly Lazy<IEnumerable<ManifestResource>> _manifestResources;
        private readonly Lazy<IEnumerable<IMemberInfo>> _memberDefinitions;
        private readonly Lazy<IEnumerable<MethodDebugInformation>> _methodDebugInformation;
        private readonly Lazy<IEnumerable<MethodDefinition>> _methodDefinitions;
        private readonly Lazy<IEnumerable<PropertyDefinition>> _propertyDefinitions;
        private readonly Lazy<IEnumerable<TypeDefinition>> _typeDefinitions;
        private readonly Lazy<IEnumerable<TypeReference>> _typeReferences;

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
                _methodDefinitions = new Lazy<IEnumerable<MethodDefinition>>(() => MetadataState.GetCodeElements(Reader.MethodDefinitions).OfType<MethodDefinition>().ToImmutableArray());
                _constructorDefinitions = new Lazy<IEnumerable<ConstructorDefinition>>(() => MetadataState.GetCodeElements(Reader.MethodDefinitions).OfType<ConstructorDefinition>().ToImmutableArray());
                _propertyDefinitions = MetadataState.GetLazyCodeElements<PropertyDefinition>(Reader.PropertyDefinitions);
                _typeDefinitions = new Lazy<IEnumerable<TypeDefinition>>(() => MetadataState.GetCodeElements<TypeDefinition>(Reader.TypeDefinitions).Where(typeDefinition => !"<Module>".Equals(typeDefinition.Name)).ToImmutableArray());
                _typeReferences = MetadataState.GetLazyCodeElements<TypeReference>(Reader.TypeReferences);
                _memberDefinitions = new Lazy<IEnumerable<IMemberInfo>>(() => MethodDefinitions.Cast<IMemberInfo>().Union(FieldDefinitions).Union(EventDefinitions).Union(PropertyDefinitions).Union(TypeDefinitions).ToImmutableArray());
                MetadataKind = Reader.MetadataKind;
            }
            else
            {
                _assemblyFiles = new Lazy<IEnumerable<AssemblyFile>>(() => ImmutableArray<AssemblyFile>.Empty);
                _declarativeSecurityAttributes = new Lazy<IEnumerable<DeclarativeSecurityAttribute>>(() => ImmutableArray<DeclarativeSecurityAttribute>.Empty);
                _eventDefinitions = new Lazy<IEnumerable<EventDefinition>>(() => ImmutableArray<EventDefinition>.Empty);
                _exportedTypes = new Lazy<IEnumerable<ExportedType>>(() => ImmutableArray<ExportedType>.Empty);
                _fieldDefinitions = new Lazy<IEnumerable<FieldDefinition>>(() => ImmutableArray<FieldDefinition>.Empty);
                _importScopes = new Lazy<IEnumerable<ImportScope>>(() => ImmutableArray<ImportScope>.Empty);
                _manifestResources = new Lazy<IEnumerable<ManifestResource>>(() => ImmutableArray<ManifestResource>.Empty);
                _methodDefinitions = new Lazy<IEnumerable<MethodDefinition>>(() => ImmutableArray<MethodDefinition>.Empty);
                _constructorDefinitions = new Lazy<IEnumerable<ConstructorDefinition>>(() => ImmutableArray<ConstructorDefinition>.Empty);
                _propertyDefinitions = new Lazy<IEnumerable<PropertyDefinition>>(() => ImmutableArray<PropertyDefinition>.Empty);
                _typeDefinitions = new Lazy<IEnumerable<TypeDefinition>>(() => ImmutableArray<TypeDefinition>.Empty);
                _typeReferences = new Lazy<IEnumerable<TypeReference>>(() => ImmutableArray<TypeReference>.Empty);
                _memberDefinitions = new Lazy<IEnumerable<IMemberInfo>>(() => ImmutableArray<IMemberInfo>.Empty);
            }
            if (MetadataState.HasDebugMetadata)
            {
                HasDebugMetadata = true;
                _customDebugInformation = MetadataState.GetLazyCodeElements<CustomDebugInformation>(MetadataState.PdbReader.CustomDebugInformation);
                _documents = MetadataState.GetLazyCodeElements<Document>(MetadataState.PdbReader.Documents);
                _languages = new Lazy<IEnumerable<Language>>(() => Documents.Select(document => document.Language).Distinct().ToImmutableArray());
                _localConstants = MetadataState.GetLazyCodeElements<LocalConstant>(MetadataState.PdbReader.LocalConstants);
                _localScopes = MetadataState.GetLazyCodeElements<LocalScope>(MetadataState.PdbReader.LocalScopes);
                _localVariables = MetadataState.GetLazyCodeElements<LocalVariable>(MetadataState.PdbReader.LocalVariables);
                _methodDebugInformation = new Lazy<IEnumerable<MethodDebugInformation>>(() =>
                {
                    // ReSharper disable once UnusedVariable -- We need to make sure all method definitions have been loaded before loading the method debug information. -- Jonathan Byrne 06/23/2017
                    var debugInformation = MethodDefinitions.Select(methodDefinition => methodDefinition.DebugInformation).ToArray();
                    return MetadataState.GetCodeElements<MethodDebugInformation>(MetadataState.PdbReader.MethodDebugInformation);
                });
            }
            else
            {
                _customDebugInformation = new Lazy<IEnumerable<CustomDebugInformation>>(() => ImmutableArray<CustomDebugInformation>.Empty);
                _documents = new Lazy<IEnumerable<Document>>(() => ImmutableArray<Document>.Empty);
                _languages = new Lazy<IEnumerable<Language>>(() => ImmutableArray<Language>.Empty);
                _localConstants = new Lazy<IEnumerable<LocalConstant>>(() => ImmutableArray<LocalConstant>.Empty);
                _localScopes = new Lazy<IEnumerable<LocalScope>>(() => ImmutableArray<LocalScope>.Empty);
                _localVariables = new Lazy<IEnumerable<LocalVariable>>(() => ImmutableArray<LocalVariable>.Empty);
                _methodDebugInformation = new Lazy<IEnumerable<MethodDebugInformation>>(() => ImmutableArray<MethodDebugInformation>.Empty);
            }
        }

        public AssemblyDefinition AssemblyDefinition { get; }

        public FileInfo AssemblyFile { get; }

        public IEnumerable<AssemblyFile> AssemblyFiles => _assemblyFiles.Value;

        public IEnumerable<AssemblyReference> AssemblyReferences => MetadataState.AssemblyReferences;

        public IEnumerable<ConstructorDefinition> ConstructorDefinitions => _constructorDefinitions.Value;

        public IEnumerable<CustomDebugInformation> CustomDebugInformation => _customDebugInformation.Value;

        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public IEnumerable<Document> Documents => _documents.Value;

        public IEnumerable<EventDefinition> EventDefinitions => !HasMetadata ? ImmutableArray<EventDefinition>.Empty : _eventDefinitions.Value;

        public IEnumerable<ExportedType> ExportedTypes => _exportedTypes.Value;

        public IEnumerable<FieldDefinition> FieldDefinitions => _fieldDefinitions.Value;

        public bool HasDebugMetadata { get; }

        public bool HasMetadata { get; }

        public IEnumerable<ImportScope> ImportScopes => _importScopes.Value;

        public Language? Language => MetadataState.Language;

        public IEnumerable<LocalConstant> LocalConstants => _localConstants.Value;

        public IEnumerable<LocalScope> LocalScopes => _localScopes.Value;

        public IEnumerable<LocalVariable> LocalVariables => _localVariables.Value;

        public IEnumerable<ManifestResource> ManifestResources => _manifestResources.Value;

        public IEnumerable<IMemberInfo> MemberDefinitions => _memberDefinitions.Value;

        public MetadataKind MetadataKind { get; }

        public IEnumerable<MethodDebugInformation> MethodDebugInformation => _methodDebugInformation.Value;

        public IEnumerable<MethodDefinition> MethodDefinitions => _methodDefinitions.Value;

        public ModuleDefinition ModuleDefinition => MetadataState.ModuleDefinition;

        public FileInfo PdbFile { get; }

        public IEnumerable<PropertyDefinition> PropertyDefinitions => _propertyDefinitions.Value;

        public IEnumerable<TypeDefinition> TypeDefinitions => _typeDefinitions.Value;

        public IEnumerable<TypeReference> TypeReferences => _typeReferences.Value;

        protected MetadataReader Reader => MetadataState.AssemblyReader ?? MetadataState.PdbReader;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;

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
