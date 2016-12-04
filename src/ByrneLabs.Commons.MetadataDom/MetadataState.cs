﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    internal class MetadataState : IDisposable
    {
        private static readonly IReadOnlyList<Type> DebugMetadataTypes = new List<Type> { typeof(CustomDebugInformationHandle), typeof(DocumentHandle), typeof(System.Reflection.Metadata.ImportDefinition), typeof(ImportScopeHandle), typeof(LocalConstantHandle), typeof(LocalScopeHandle), typeof(LocalVariableHandle), typeof(MethodDebugInformationHandle) };
        private static readonly IReadOnlyDictionary<Type, Type> HandleTypeMap = new ReadOnlyDictionary<Type, Type>(new Dictionary<Type, Type>
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
            { typeof(DocumentNameBlobHandle), typeof(DocumentNameBlob) },
            { typeof(EventDefinitionHandle), typeof(EventDefinition) },
            { typeof(ExportedTypeHandle), typeof(ExportedType) },
            { typeof(FieldDefinitionHandle), typeof(FieldDefinition) },
            { typeof(GenericParameterHandle), typeof(GenericParameter) },
            { typeof(GenericParameterConstraintHandle), typeof(GenericParameterConstraint) },
            { typeof(System.Reflection.Metadata.ImportDefinition), typeof(ImportDefinition) },
            { typeof(ImportScopeHandle), typeof(ImportScope) },
            { typeof(InterfaceImplementationHandle), typeof(InterfaceImplementation) },
            { typeof(LocalConstantHandle), typeof(LocalConstant) },
            { typeof(LocalScopeHandle), typeof(LocalScope) },
            { typeof(LocalVariableHandle), typeof(LocalVariable) },
            { typeof(ManifestResourceHandle), typeof(ManifestResource) },
            { typeof(MemberReferenceHandle), typeof(MemberReference) },
            { typeof(MethodDebugInformationHandle), typeof(MethodDebugInformation) },
            { typeof(MethodDefinitionHandle), typeof(MethodDefinition) },
            { typeof(MethodImplementationHandle), typeof(MethodImplementation) },
            { typeof(System.Reflection.Metadata.MethodImport), typeof(MethodImport) },
            { typeof(MethodSpecificationHandle), typeof(MethodSpecification) },
            { typeof(ModuleDefinitionHandle), typeof(ModuleDefinition) },
            { typeof(ModuleReferenceHandle), typeof(ModuleReference) },
            { typeof(NamespaceDefinitionHandle), typeof(NamespaceDefinition) },
            { typeof(ParameterHandle), typeof(Parameter) },
            { typeof(PropertyDefinitionHandle), typeof(PropertyDefinition) },
            { typeof(System.Reflection.Metadata.SequencePoint), typeof(SequencePoint) },
            { typeof(StandaloneSignatureHandle), typeof(StandaloneSignature) },
            { typeof(TypeDefinitionHandle), typeof(TypeDefinition) },
            { typeof(TypeReferenceHandle), typeof(TypeReference) },
            { typeof(TypeSpecificationHandle), typeof(TypeSpecification) }
        });
        private readonly MetadataFile _assemblyFile;
        private readonly IDictionary<Handle, ICodeElementWithHandle> _codeElementWithHandleCache = new Dictionary<Handle, ICodeElementWithHandle>();
        private readonly IDictionary<object, ICodeElementWithoutHandle> _codeElementWithoutHandleCache = new Dictionary<object, ICodeElementWithoutHandle>();
        private readonly MetadataFile _pdbFile;
        private AssemblyDefinition _assemblyDefinition;
        private ModuleDefinition _moduleDefinition;

        public MetadataState(bool prefetchMetadata, FileInfo assemblyFile, FileInfo pdbFile)
        {
            if (assemblyFile?.Exists == true)
            {
                _assemblyFile = new MetadataFile(prefetchMetadata, assemblyFile, MetadataFileType.Assembly);
            }

            if (pdbFile?.Exists == true)
            {
                _pdbFile = new MetadataFile(prefetchMetadata, pdbFile, MetadataFileType.Pdb);
            }
        }

        public MetadataReader AssemblyReader => _assemblyFile?.Reader;

        public bool HasDebugMetadata => _pdbFile?.HasMetadata == true;

        public bool HasMetadata => _assemblyFile?.HasMetadata == true;

        public MetadataReader PdbReader => _pdbFile?.Reader;

        public void Dispose()
        {
            _pdbFile?.Dispose();
            _assemblyFile?.Dispose();
        }

        [SuppressMessage("ReSharper", "CyclomaticComplexity", Justification = "There is no obvious way to reduce the cyclomatic complexity of this method")]
        public static Handle? DowncastHandle(object handle)
        {
            Handle? downcastHandle;
            if (handle is AssemblyDefinitionHandle)
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

        [SuppressMessage("ReSharper", "CyclomaticComplexity", Justification = "There is no obvious way to reduce the cyclomatic complexity of this method")]
        private static object UpcastHandle(Handle handle)
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

        public void CacheCodeElement(CodeElement codeElement)
        {
            var codeElementWithHandle = codeElement as ICodeElementWithHandle;
            var codeElementWithoutHandle = codeElement as ICodeElementWithoutHandle;
            var assemblyDefinition = codeElement as AssemblyDefinition;
            var moduleDefinition = codeElement as ModuleDefinition;
            if (assemblyDefinition != null && codeElementWithHandle.MetadataHandle is AssemblyDefinitionHandle)
            {
                _assemblyDefinition = assemblyDefinition;
            }
            else if (moduleDefinition != null && codeElementWithHandle.MetadataHandle is ModuleDefinitionHandle)
            {
                _moduleDefinition = moduleDefinition;
            }
            else if (codeElementWithHandle != null)
            {
                _codeElementWithHandleCache.Add(codeElementWithHandle.DowncastMetadataHandle.Value, codeElementWithHandle);
            }
            else if (codeElementWithoutHandle != null)
            {
                _codeElementWithoutHandleCache.Add(codeElementWithoutHandle.MetadataKey, codeElementWithoutHandle);
            }
            else
            {
                throw new ArgumentException($"Invalid code element type {codeElement.GetType()}", nameof(codeElement));
            }
        }

        public T GetCodeElement<T>(object handle) where T : CodeElement => (T) GetCodeElement(handle);

        public CodeElement GetCodeElement(object handle)
        {
            CodeElement codeElement;
            var downcastHandle = DowncastHandle(handle);
            if (downcastHandle == null)
            {
                if (_codeElementWithoutHandleCache.ContainsKey(handle))
                {
                    codeElement = (CodeElement) _codeElementWithoutHandleCache[handle];
                }
                else
                {
                    var codeElementType = HandleTypeMap[handle.GetType()];
                    var constructor = codeElementType.GetTypeInfo().DeclaredConstructors.Single(constructorCheck => constructorCheck.GetParameters().Length == 2 && constructorCheck.GetParameters()[1].ParameterType == GetType());
                    codeElement = (CodeElement) constructor.Invoke(new[] { handle, this });
                }
            }
            else if (downcastHandle.Value.IsNil)
            {
                codeElement = null;
            }
            else if (handle is AssemblyDefinitionHandle)
            {
                codeElement = _assemblyDefinition ?? (_assemblyDefinition = new AssemblyDefinition((AssemblyDefinitionHandle) handle, this));
            }
            else if (handle is ModuleDefinitionHandle)
            {
                codeElement = _moduleDefinition ?? (_moduleDefinition = new ModuleDefinition((ModuleDefinitionHandle) handle, this));
            }
            else if (DebugMetadataTypes.Contains(handle.GetType()) && !HasDebugMetadata || !DebugMetadataTypes.Contains(handle.GetType()) && !HasMetadata)
            {
                codeElement = null;
            }
            else
            {
                if (_codeElementWithHandleCache.ContainsKey(downcastHandle.Value))
                {
                    codeElement = (CodeElement) _codeElementWithHandleCache[downcastHandle.Value];
                }
                else
                {
                    var upcastHandle = UpcastHandle(downcastHandle.Value);
                    var codeElementType = HandleTypeMap[upcastHandle.GetType()];
                    var constructor = codeElementType.GetTypeInfo().DeclaredConstructors.Single(constructorCheck => constructorCheck.GetParameters().Length == 2 && constructorCheck.GetParameters()[1].ParameterType == GetType());
                    codeElement = (CodeElement) constructor.Invoke(new[] { upcastHandle, this });
                }
            }

            return codeElement;
        }

        public IReadOnlyList<T> GetCodeElements<T>(IEnumerable handles) where T : CodeElement => handles == null ? new List<T>() : handles.Cast<object>().Select(GetCodeElement<T>).ToList();
    }
}
