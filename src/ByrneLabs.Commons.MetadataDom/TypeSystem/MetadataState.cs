﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    internal class MetadataState : IDisposable
    {
        private static readonly IEnumerable<Type> DebugMetadataTypes = new List<Type> { typeof(CustomDebugInformationHandle), typeof(DocumentHandle), typeof(ImportDefinition), typeof(ImportScopeHandle), typeof(LocalConstantHandle), typeof(LocalScopeHandle), typeof(LocalVariableHandle), typeof(MethodDebugInformationHandle) };
        private static readonly IDictionary<Type, Type> HandleTypeMap = new Dictionary<Type, Type>(new Dictionary<Type, Type>
        {
            { typeof(AssemblyDefinitionHandle), typeof(AssemblyDefinition) },
            { typeof(AssemblyFileHandle), typeof(AssemblyFile) },
            { typeof(AssemblyReferenceHandle), typeof(AssemblyReference) },
            { typeof(BlobHandle), typeof(Blob) },
            { typeof(ConstantHandle), typeof(Constant) },
            { typeof(CustomAttributeHandle), typeof(CustomAttribute) },
            { typeof(CustomDebugInformationHandle), typeof(CustomDebugInformation) },
            { typeof(DeclarativeSecurityAttributeHandle), typeof(DeclarativeSecurityAttribute) },
            { typeof(DocumentHandle), typeof(Document) },
            { typeof(EventDefinitionHandle), typeof(EventDefinition) },
            { typeof(ExportedTypeHandle), typeof(ExportedType) },
            { typeof(FieldDefinitionHandle), typeof(FieldDefinition) },
            { typeof(GenericParameterHandle), typeof(GenericParameter) },
            { typeof(GenericParameterConstraintHandle), typeof(GenericParameterConstraint) },
            { typeof(ImportDefinition), typeof(ImportDefinition) },
            { typeof(ImportScopeHandle), typeof(ImportScope) },
            { typeof(InterfaceImplementationHandle), typeof(InterfaceImplementation) },
            { typeof(LocalConstantHandle), typeof(LocalConstant) },
            { typeof(LocalScopeHandle), typeof(LocalScope) },
            { typeof(LocalVariableHandle), typeof(LocalVariable) },
            { typeof(ManifestResourceHandle), typeof(ManifestResource) },
            { typeof(MemberReferenceHandle), typeof(MemberReference) },
            { typeof(MethodDebugInformationHandle), typeof(System.Reflection.Metadata.MethodDebugInformation) },
            { typeof(MethodDefinitionHandle), typeof(MethodDefinition) },
            { typeof(MethodImplementationHandle), typeof(System.Reflection.Metadata.MethodImplementation) },
            { typeof(MethodImport), typeof(MethodImport) },
            { typeof(MethodSpecificationHandle), typeof(MethodSpecification) },
            { typeof(ModuleDefinitionHandle), typeof(ModuleDefinition) },
            { typeof(ModuleReferenceHandle), typeof(ModuleReference) },
            { typeof(NamespaceDefinitionHandle), typeof(NamespaceDefinition) },
            { typeof(ParameterHandle), typeof(Parameter) },
            { typeof(PropertyDefinitionHandle), typeof(PropertyDefinition) },
            { typeof(System.Reflection.Metadata.SequencePoint), typeof(System.Reflection.Metadata.SequencePoint) },
            { typeof(StandaloneSignatureHandle), typeof(StandaloneSignature) },
            { typeof(TypeDefinitionHandle), typeof(TypeDefinition) },
            { typeof(TypeReferenceHandle), typeof(TypeReference) },
            { typeof(TypeSpecificationHandle), typeof(TypeSpecification) }
        });
        private readonly Lazy<AssemblyDefinition> _assemblyDefinition;
        private readonly Lazy<IEnumerable<AssemblyReference>> _assemblyReferences;
        private readonly IDictionary<CodeElementKey, IManagedCodeElement> _codeElementCache = new Dictionary<CodeElementKey, IManagedCodeElement>();
        private readonly Lazy<IEnumerable<TypeInfo>> _definedTypes;
        private readonly Lazy<Language?> _language;
        private readonly Lazy<ModuleDefinition> _moduleDefinition;

        public MetadataState(bool prefetchMetadata, FileInfo assemblyFile, FileInfo pdbFile)
        {
            TypeProvider = new TypeProvider(this);
            if (assemblyFile != null)
            {
                if (!assemblyFile.Exists)
                {
                    throw new ArgumentException($"The file {assemblyFile.FullName} does not exist", nameof(assemblyFile));
                }

                AssemblyFileWrapper = new CompiledFileWrapper(prefetchMetadata, assemblyFile);
            }

            if (AssemblyFileWrapper != null || pdbFile != null)
            {
                try
                {
                    PdbFileWrapper = new CompiledFileWrapper(prefetchMetadata, AssemblyFileWrapper, pdbFile);
                }
                catch (BadImageFormatException exception)
                {
                    if (!exception.Message.Equals("Invalid COR20 header signature."))
                    {
                        throw;
                    }
                }
            }

            _assemblyReferences = new Lazy<IEnumerable<AssemblyReference>>(() => AssemblyReader == null ? Enumerable.Empty<AssemblyReference>() : GetCodeElements<AssemblyReference>(AssemblyReader.AssemblyReferences));
            _assemblyDefinition = new Lazy<AssemblyDefinition>(() => AssemblyReader?.IsAssembly == true ? GetCodeElement<AssemblyDefinition>(Handle.AssemblyDefinition) : null);
            _moduleDefinition = GetLazyCodeElement<ModuleDefinition>(Handle.ModuleDefinition);
            _definedTypes = new Lazy<IEnumerable<TypeInfo>>(() => AssemblyReader == null ? ImmutableArray<TypeInfo>.Empty : AssemblyReader.TypeDefinitions.Select(typeDefinition => GetCodeElement(typeDefinition)).Cast<TypeInfo>().Where(type => !"<Module>".Equals(type.FullName)).ToImmutableArray());
            _language = new Lazy<Language?>(() =>
            {
                var languageGuid = PdbReader?.Documents.Select(documentHandle => PdbReader.GetDocument(documentHandle)).Where(document => !document.Language.IsNil).Select(document => PdbReader.GetGuid(document.Language)).FirstOrDefault();
                return languageGuid.HasValue ? (Language?) KnownLanguageGuids.MapFromGuid[languageGuid.Value] : null;
            });
        }

        public AssemblyDefinition AssemblyDefinition => _assemblyDefinition.Value;

        public MetadataReader AssemblyReader => AssemblyFileWrapper?.Reader;

        public IEnumerable<AssemblyReference> AssemblyReferences => _assemblyReferences.Value;

        public IEnumerable<TypeInfo> DefinedTypes => !HasMetadata ? ImmutableArray<TypeInfo>.Empty : _definedTypes.Value;

        public bool HasDebugMetadata => PdbFileWrapper?.HasMetadata == true;

        public bool HasMetadata => AssemblyFileWrapper?.HasMetadata == true;

        public Language? Language => _language.Value;

        public ModuleDefinition ModuleDefinition => !HasMetadata ? null : _moduleDefinition.Value;

        public MetadataReader PdbReader => PdbFileWrapper?.Reader;

        internal CompiledFileWrapper AssemblyFileWrapper { get; }

        internal CompiledFileWrapper PdbFileWrapper { get; }

        internal TypeProvider TypeProvider { get; }

        [SuppressMessage("ReSharper", "CyclomaticComplexity", Justification = "There is no obvious way to reduce the cyclomatic complexity of this method")]
        public static Handle? DowncastHandle(object handle)
        {
            Handle? downcastHandle;
            if (handle is Handle)
            {
                downcastHandle = (Handle) handle;
            }
            else if (handle is AssemblyDefinitionHandle)
            {
                downcastHandle = (AssemblyDefinitionHandle) handle;
            }
            else if (handle is AssemblyFileHandle)
            {
                downcastHandle = (AssemblyFileHandle) handle;
            }
            else if (handle is AssemblyReferenceHandle)
            {
                downcastHandle = (AssemblyReferenceHandle) handle;
            }
            else if (handle is BlobHandle)
            {
                downcastHandle = (BlobHandle) handle;
            }
            else if (handle is ConstantHandle)
            {
                downcastHandle = (ConstantHandle) handle;
            }
            else if (handle is CustomAttributeHandle)
            {
                downcastHandle = (CustomAttributeHandle) handle;
            }
            else if (handle is CustomDebugInformationHandle)
            {
                downcastHandle = (CustomDebugInformationHandle) handle;
            }
            else if (handle is DeclarativeSecurityAttributeHandle)
            {
                downcastHandle = (DeclarativeSecurityAttributeHandle) handle;
            }
            else if (handle is DocumentHandle)
            {
                downcastHandle = (DocumentHandle) handle;
            }
            else if (handle is DocumentNameBlobHandle)
            {
                BlobHandle blobHandle = (DocumentNameBlobHandle) handle;
                downcastHandle = blobHandle;
            }
            else if (handle is EventDefinitionHandle)
            {
                downcastHandle = (EventDefinitionHandle) handle;
            }
            else if (handle is ExportedTypeHandle)
            {
                downcastHandle = (ExportedTypeHandle) handle;
            }
            else if (handle is FieldDefinitionHandle)
            {
                downcastHandle = (FieldDefinitionHandle) handle;
            }
            else if (handle is GenericParameterHandle)
            {
                downcastHandle = (GenericParameterHandle) handle;
            }
            else if (handle is GenericParameterConstraintHandle)
            {
                downcastHandle = (GenericParameterConstraintHandle) handle;
            }
            else if (handle is ImportScopeHandle)
            {
                downcastHandle = (ImportScopeHandle) handle;
            }
            else if (handle is InterfaceImplementationHandle)
            {
                downcastHandle = (InterfaceImplementationHandle) handle;
            }
            else if (handle is LocalConstantHandle)
            {
                downcastHandle = (LocalConstantHandle) handle;
            }
            else if (handle is LocalScopeHandle)
            {
                downcastHandle = (LocalScopeHandle) handle;
            }
            else if (handle is LocalVariableHandle)
            {
                downcastHandle = (LocalVariableHandle) handle;
            }
            else if (handle is ManifestResourceHandle)
            {
                downcastHandle = (ManifestResourceHandle) handle;
            }
            else if (handle is MemberReferenceHandle)
            {
                downcastHandle = (MemberReferenceHandle) handle;
            }
            else if (handle is MethodDebugInformationHandle)
            {
                downcastHandle = (MethodDebugInformationHandle) handle;
            }
            else if (handle is MethodDefinitionHandle)
            {
                downcastHandle = (MethodDefinitionHandle) handle;
            }
            else if (handle is MethodImplementationHandle)
            {
                downcastHandle = (MethodImplementationHandle) handle;
            }
            else if (handle is MethodSpecificationHandle)
            {
                downcastHandle = (MethodSpecificationHandle) handle;
            }
            else if (handle is ModuleDefinitionHandle)
            {
                downcastHandle = (ModuleDefinitionHandle) handle;
            }
            else if (handle is ModuleReferenceHandle)
            {
                downcastHandle = (ModuleReferenceHandle) handle;
            }
            else if (handle is NamespaceDefinitionHandle)
            {
                downcastHandle = (NamespaceDefinitionHandle) handle;
            }
            else if (handle is ParameterHandle)
            {
                downcastHandle = (ParameterHandle) handle;
            }
            else if (handle is PropertyDefinitionHandle)
            {
                downcastHandle = (PropertyDefinitionHandle) handle;
            }
            else if (handle is StandaloneSignatureHandle)
            {
                downcastHandle = (StandaloneSignatureHandle) handle;
            }
            else if (handle is TypeDefinitionHandle)
            {
                downcastHandle = (TypeDefinitionHandle) handle;
            }
            else if (handle is TypeReferenceHandle)
            {
                downcastHandle = (TypeReferenceHandle) handle;
            }
            else if (handle is TypeSpecificationHandle)
            {
                downcastHandle = (TypeSpecificationHandle) handle;
            }
            else if (handle is EntityHandle)
            {
                downcastHandle = (EntityHandle) handle;
            }
            else
            {
                downcastHandle = null;
            }

            return downcastHandle;
        }

        public static Type GetCodeElementTypeForHandle(Handle handle)
        {
            var upcastHandle = UpcastHandle(handle);
            return HandleTypeMap[upcastHandle.GetType()];
        }

        [SuppressMessage("ReSharper", "CyclomaticComplexity", Justification = "There is no obvious way to reduce the cyclomatic complexity of this method")]
        public static object UpcastHandle(Handle handle)
        {
            object upcastHandle;
            switch (handle.Kind)
            {
                case HandleKind.AssemblyDefinition:
                    upcastHandle = (AssemblyDefinitionHandle) handle;
                    break;
                case HandleKind.AssemblyFile:
                    upcastHandle = (AssemblyFileHandle) handle;
                    break;
                case HandleKind.AssemblyReference:
                    upcastHandle = (AssemblyReferenceHandle) handle;
                    break;
                case HandleKind.Blob:
                    upcastHandle = (BlobHandle) handle;
                    break;
                case HandleKind.Constant:
                    upcastHandle = (ConstantHandle) handle;
                    break;
                case HandleKind.CustomAttribute:
                    upcastHandle = (CustomAttributeHandle) handle;
                    break;
                case HandleKind.CustomDebugInformation:
                    upcastHandle = (CustomDebugInformationHandle) handle;
                    break;
                case HandleKind.DeclarativeSecurityAttribute:
                    upcastHandle = (DeclarativeSecurityAttributeHandle) handle;
                    break;
                case HandleKind.Document:
                    upcastHandle = (DocumentHandle) handle;
                    break;
                case HandleKind.EventDefinition:
                    upcastHandle = (EventDefinitionHandle) handle;
                    break;
                case HandleKind.ExportedType:
                    upcastHandle = (ExportedTypeHandle) handle;
                    break;
                case HandleKind.FieldDefinition:
                    upcastHandle = (FieldDefinitionHandle) handle;
                    break;
                case HandleKind.GenericParameter:
                    upcastHandle = (GenericParameterHandle) handle;
                    break;
                case HandleKind.GenericParameterConstraint:
                    upcastHandle = (GenericParameterConstraintHandle) handle;
                    break;
                case HandleKind.Guid:
                    upcastHandle = (GuidHandle) handle;
                    break;
                case HandleKind.ImportScope:
                    upcastHandle = (ImportScopeHandle) handle;
                    break;
                case HandleKind.InterfaceImplementation:
                    upcastHandle = (InterfaceImplementationHandle) handle;
                    break;
                case HandleKind.LocalConstant:
                    upcastHandle = (LocalConstantHandle) handle;
                    break;
                case HandleKind.LocalScope:
                    upcastHandle = (LocalScopeHandle) handle;
                    break;
                case HandleKind.LocalVariable:
                    upcastHandle = (LocalVariableHandle) handle;
                    break;
                case HandleKind.ManifestResource:
                    upcastHandle = (ManifestResourceHandle) handle;
                    break;
                case HandleKind.MemberReference:
                    upcastHandle = (MemberReferenceHandle) handle;
                    break;
                case HandleKind.MethodDebugInformation:
                    upcastHandle = (MethodDebugInformationHandle) handle;
                    break;
                case HandleKind.MethodDefinition:
                    upcastHandle = (MethodDefinitionHandle) handle;
                    break;
                case HandleKind.MethodImplementation:
                    upcastHandle = (MethodImplementationHandle) handle;
                    break;
                case HandleKind.MethodSpecification:
                    upcastHandle = (MethodSpecificationHandle) handle;
                    break;
                case HandleKind.ModuleDefinition:
                    upcastHandle = (ModuleDefinitionHandle) handle;
                    break;
                case HandleKind.ModuleReference:
                    upcastHandle = (ModuleReferenceHandle) handle;
                    break;
                case HandleKind.NamespaceDefinition:
                    upcastHandle = (NamespaceDefinitionHandle) handle;
                    break;
                case HandleKind.Parameter:
                    upcastHandle = (ParameterHandle) handle;
                    break;
                case HandleKind.PropertyDefinition:
                    upcastHandle = (PropertyDefinitionHandle) handle;
                    break;
                case HandleKind.StandaloneSignature:
                    upcastHandle = (StandaloneSignatureHandle) handle;
                    break;
                case HandleKind.String:
                    upcastHandle = (StringHandle) handle;
                    break;
                case HandleKind.TypeDefinition:
                    upcastHandle = (TypeDefinitionHandle) handle;
                    break;
                case HandleKind.TypeReference:
                    upcastHandle = (TypeReferenceHandle) handle;
                    break;
                case HandleKind.TypeSpecification:
                    upcastHandle = (TypeSpecificationHandle) handle;
                    break;
                case HandleKind.UserString:
                    upcastHandle = (UserStringHandle) handle;
                    break;
                default:
                    throw new ArgumentException($"Invalid handle kind {handle.Kind}", nameof(handle));
            }

            return upcastHandle;
        }

        private static System.Reflection.ConstructorInfo GetConstructor(Type type, IReadOnlyList<object> parameters)
        {
            System.Reflection.ConstructorInfo matchingConstructor = null;
            foreach (var constructor in type.GetTypeInfo().DeclaredConstructors.Where(constructor => constructor.GetParameters().Length == parameters.Count))
            {
                var match = true;
                var parameterIndex = 0;
                foreach (var parameter in constructor.GetParameters())
                {
                    if (parameters[parameterIndex] != null && !parameter.ParameterType.GetTypeInfo().IsInstanceOfType(parameters[parameterIndex]))
                    {
                        match = false;
                        break;
                    }

                    parameterIndex++;
                }

                if (match)
                {
                    matchingConstructor = constructor;
                    break;
                }
            }

            return matchingConstructor;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public AssemblyReference FindAssemblyReference(AssemblyName assemblyName) => AssemblyReferences.SingleOrDefault(assemblyReference => assemblyReference.FullName.Equals(assemblyName.FullName));

        public IManagedCodeElement GetCodeElement(Type codeElementType, params object[] keyValues) => GetCodeElement(new CodeElementKey(codeElementType, keyValues));

        public IManagedCodeElement GetCodeElement(object handle) => GetCodeElement(new CodeElementKey(handle));

        public IManagedCodeElement GetCodeElement(CodeElementKey key)
        {
            IManagedCodeElement codeElement;
            if (_codeElementCache.ContainsKey(key))
            {
                codeElement = _codeElementCache[key];
            }
            else if (key.Handle?.IsNil == true || DebugMetadataTypes.Contains(key.UpcastHandle?.GetType()) && !HasDebugMetadata || !DebugMetadataTypes.Contains(key.UpcastHandle?.GetType()) && !HasMetadata)
            {
                codeElement = null;
            }
            else
            {
                Type codeElementType;
                if (key.UpcastHandle is MethodDefinitionHandle)
                {
                    var methodDefinition = AssemblyReader.GetMethodDefinition((MethodDefinitionHandle) key.UpcastHandle);
                    var methodName = AssemblyReader.GetString(methodDefinition.Name);
                    if (".ctor".Equals(methodName) || ".cctor".Equals(methodName))
                    {
                        codeElementType = typeof(ConstructorDefinition);
                    }
                    else
                    {
                        codeElementType = typeof(MethodDefinition);
                    }
                }
                else if (key.UpcastHandle is MemberReferenceHandle)
                {
                    var memberReference = AssemblyReader.GetMemberReference((MemberReferenceHandle) key.UpcastHandle);
                    var methodName = AssemblyReader.GetString(memberReference.Name);
                    if (memberReference.GetKind() == MemberReferenceKind.Field)
                    {
                        codeElementType = typeof(FieldReference);
                    }
                    else if (".ctor".Equals(methodName) || ".cctor".Equals(methodName))
                    {
                        codeElementType = typeof(ConstructorReference);
                    }
                    else
                    {
                        codeElementType = typeof(MethodReference);
                    }
                }
                else
                {
                    codeElementType = key.CodeElementType;
                }
                var constructorParameterValues = key.KeyValues.Select(keyValue => keyValue is Handle ? UpcastHandle((Handle) keyValue) : keyValue).Union(key.ExtraConstructorArguments).Append(this).ToArray();
                var constructor = GetConstructor(codeElementType, constructorParameterValues);
                if (constructor == null)
                {
                    throw new ArgumentException($"The key values provided are invalid for creating a {codeElementType.Name} code element");
                }

                codeElement = (IManagedCodeElement) constructor.Invoke(constructorParameterValues);
                if (_codeElementCache.ContainsKey(codeElement.Key))
                {
                    throw new InvalidOperationException("The code element was already added with a different key");
                }

                _codeElementCache.Add(codeElement.Key, codeElement);
                /*
                 * Elements should create the same key for themselves as we create here but there is no issue with indexing on both keys just to be certain. -- Jonathan Byrne 07/10/2017
                 */
                if (!codeElement.Key.Equals(key))
                {
                    if (_codeElementCache.ContainsKey(key))
                    {
                        throw new InvalidOperationException("The code element was already added with a different key");
                    }

                    _codeElementCache.Add(key, codeElement);
                }
            }

            return codeElement;
        }

        public T GetCodeElement<T>(CodeElementKey key) => (T) GetCodeElement(key);

        public T GetCodeElement<T>(params object[] keyValues) => (T) GetCodeElement(new CodeElementKey(typeof(T), keyValues));

        public IEnumerable<IManagedCodeElement> GetCodeElements(IEnumerable handles) => handles.Cast<object>().Select(GetCodeElement).ToImmutableArray();

        public IEnumerable<T> GetCodeElements<T>(IEnumerable handles) => handles.Cast<object>().Select(handle => GetCodeElement<T>(handle)).ToImmutableArray();

        public T GetCodeElementWithConstructorArguments<T>(IEnumerable<object> keyValues, IEnumerable<object> extraConstructorArguments) => (T) GetCodeElement(new CodeElementKey(typeof(T), keyValues, extraConstructorArguments));

        public Lazy<IManagedCodeElement> GetLazyCodeElement(object handle) => new Lazy<IManagedCodeElement>(() => GetCodeElement(handle));

        public Lazy<T> GetLazyCodeElement<T>(object handle) => new Lazy<T>(() => GetCodeElement<T>(handle));

        public Lazy<T> GetLazyCodeElement<T>(params object[] keyValues) => new Lazy<T>(() => (T) (object) GetCodeElement(new CodeElementKey(typeof(T), keyValues)));

        public Lazy<IEnumerable<T>> GetLazyCodeElements<T>(IEnumerable handles) => new Lazy<IEnumerable<T>>(() => GetCodeElements<T>(handles));

        public Lazy<IEnumerable<TReturn>> GetLazyCodeElements<TElement, TReturn>(IEnumerable handles) where TElement : TReturn => new Lazy<IEnumerable<TReturn>>(() => GetCodeElements<TElement>(handles).Cast<TReturn>().ToImmutableArray());

        public MethodBodyBlock GetMethodBodyBlock(int relativeVirtualAddress)
        {
            MethodBodyBlock methodBodyBlock;
            if (relativeVirtualAddress == 0)
            {
                methodBodyBlock = null;
            }
            else
            {
                methodBodyBlock = AssemblyFileWrapper.PEReader.GetMethodBody(relativeVirtualAddress);
            }

            return methodBodyBlock;
        }

        [SuppressMessage("ReSharper", "CyclomaticComplexity", Justification = "There is no obvious way to reduce the cyclomatic complexity of this method")]
        public object GetRawMetadataForHandle(object handle)
        {
            object token;
            var upcastHandle = handle is Handle ? UpcastHandle((Handle) handle) : handle;
            if (upcastHandle == null)
            {
                token = null;
            }
            else if (upcastHandle is AssemblyDefinitionHandle)
            {
                token = AssemblyReader.GetAssemblyDefinition();
            }
            else if (upcastHandle is AssemblyFileHandle)
            {
                token = AssemblyReader.GetAssemblyFile((AssemblyFileHandle) upcastHandle);
            }
            else if (upcastHandle is AssemblyReferenceHandle)
            {
                token = AssemblyReader.GetAssemblyReference((AssemblyReferenceHandle) upcastHandle);
            }
            else if (upcastHandle is ConstantHandle)
            {
                token = AssemblyReader.GetConstant((ConstantHandle) upcastHandle);
            }
            else if (upcastHandle is CustomAttributeHandle)
            {
                token = AssemblyReader.GetCustomAttribute((CustomAttributeHandle) upcastHandle);
            }
            else if (upcastHandle is CustomDebugInformationHandle)
            {
                token = AssemblyReader.GetCustomDebugInformation((CustomDebugInformationHandle) upcastHandle);
            }
            else if (upcastHandle is DeclarativeSecurityAttributeHandle)
            {
                token = AssemblyReader.GetDeclarativeSecurityAttribute((DeclarativeSecurityAttributeHandle) upcastHandle);
            }
            else if (upcastHandle is DocumentHandle)
            {
                token = AssemblyReader.GetDocument((DocumentHandle) upcastHandle);
            }
            else if (upcastHandle is EventDefinitionHandle)
            {
                token = AssemblyReader.GetEventDefinition((EventDefinitionHandle) upcastHandle);
            }
            else if (upcastHandle is ExportedTypeHandle)
            {
                token = AssemblyReader.GetExportedType((ExportedTypeHandle) upcastHandle);
            }
            else if (upcastHandle is FieldDefinitionHandle)
            {
                token = AssemblyReader.GetFieldDefinition((FieldDefinitionHandle) upcastHandle);
            }
            else if (upcastHandle is GenericParameterHandle)
            {
                token = AssemblyReader.GetGenericParameter((GenericParameterHandle) upcastHandle);
            }
            else if (upcastHandle is GenericParameterConstraintHandle)
            {
                token = AssemblyReader.GetGenericParameterConstraint((GenericParameterConstraintHandle) upcastHandle);
            }
            else if (upcastHandle is ImportScopeHandle)
            {
                token = AssemblyReader.GetImportScope((ImportScopeHandle) upcastHandle);
            }
            else if (upcastHandle is InterfaceImplementationHandle)
            {
                token = AssemblyReader.GetInterfaceImplementation((InterfaceImplementationHandle) upcastHandle);
            }
            else if (upcastHandle is LocalConstantHandle)
            {
                token = AssemblyReader.GetLocalConstant((LocalConstantHandle) upcastHandle);
            }
            else if (upcastHandle is LocalScopeHandle)
            {
                token = AssemblyReader.GetLocalScope((LocalScopeHandle) upcastHandle);
            }
            else if (upcastHandle is LocalVariableHandle)
            {
                token = AssemblyReader.GetLocalVariable((LocalVariableHandle) upcastHandle);
            }
            else if (upcastHandle is ManifestResourceHandle)
            {
                token = AssemblyReader.GetManifestResource((ManifestResourceHandle) upcastHandle);
            }
            else if (upcastHandle is MemberReferenceHandle)
            {
                token = AssemblyReader.GetMemberReference((MemberReferenceHandle) upcastHandle);
            }
            else if (upcastHandle is MethodDebugInformationHandle)
            {
                token = AssemblyReader.GetMethodDebugInformation((MethodDebugInformationHandle) upcastHandle);
            }
            else if (upcastHandle is MethodDefinitionHandle)
            {
                token = AssemblyReader.GetMethodDefinition((MethodDefinitionHandle) upcastHandle);
            }
            else if (upcastHandle is MethodImplementationHandle)
            {
                token = AssemblyReader.GetMethodImplementation((MethodImplementationHandle) upcastHandle);
            }
            else if (upcastHandle is MethodSpecificationHandle)
            {
                token = AssemblyReader.GetMethodSpecification((MethodSpecificationHandle) upcastHandle);
            }
            else if (upcastHandle is ModuleDefinitionHandle)
            {
                token = AssemblyReader.GetModuleDefinition();
            }
            else if (upcastHandle is ModuleReferenceHandle)
            {
                token = AssemblyReader.GetModuleReference((ModuleReferenceHandle) upcastHandle);
            }
            else if (upcastHandle is NamespaceDefinitionHandle)
            {
                token = AssemblyReader.GetNamespaceDefinition((NamespaceDefinitionHandle) upcastHandle);
            }
            else if (upcastHandle is ParameterHandle)
            {
                token = AssemblyReader.GetParameter((ParameterHandle) upcastHandle);
            }
            else if (upcastHandle is PropertyDefinitionHandle)
            {
                token = AssemblyReader.GetPropertyDefinition((PropertyDefinitionHandle) upcastHandle);
            }
            else if (upcastHandle is StandaloneSignatureHandle)
            {
                token = AssemblyReader.GetStandaloneSignature((StandaloneSignatureHandle) upcastHandle);
            }
            else if (upcastHandle is TypeDefinitionHandle)
            {
                token = AssemblyReader.GetTypeDefinition((TypeDefinitionHandle) upcastHandle);
            }
            else if (upcastHandle is TypeReferenceHandle)
            {
                token = AssemblyReader.GetTypeReference((TypeReferenceHandle) upcastHandle);
            }
            else if (upcastHandle is TypeSpecificationHandle)
            {
                token = AssemblyReader.GetTypeSpecification((TypeSpecificationHandle) upcastHandle);
            }
            else
            {
                throw new ArgumentException($"{handle.GetType()} is an invalid handle type", nameof(handle));
            }

            return token;
        }

        protected virtual void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                PdbFileWrapper?.Dispose();
                AssemblyFileWrapper?.Dispose();
            }
        }
    }
}
