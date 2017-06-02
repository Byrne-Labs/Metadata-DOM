using System;
using System.IO;
using System.Reflection;
using ByrneLabs.Commons.MetadataDom.TypeSystem;
using JetBrains.Annotations;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using MethodInfoToExpose = System.Reflection.MethodInfo;
using ModuleToExpose = System.Reflection.Module;
using AssemblyToExpose = System.Reflection.Assembly;

#else
using System.Linq;
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeInfoToExpose = ByrneLabs.Commons.MetadataDom.TypeInfo;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using MethodInfoToExpose = ByrneLabs.Commons.MetadataDom.MethodInfo;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;
using AssemblyToExpose = ByrneLabs.Commons.MetadataDom.Assembly;

#endif

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract partial class Assembly
    {
        public static AssemblyToExpose LoadMetadata(FileInfo assemblyFile) => LoadMetadata(assemblyFile, null, true);

        public static AssemblyToExpose LoadMetadata(FileInfo assemblyFile, FileInfo pdbFile) => LoadMetadata(assemblyFile, pdbFile, true);

        public static AssemblyToExpose LoadMetadata(string assemblyFileName) => LoadMetadata(assemblyFileName, null, true);

        public static AssemblyToExpose LoadMetadata(string assemblyFileName, string pdbFileName) => LoadMetadata(assemblyFileName, pdbFileName, true);

        public static AssemblyToExpose LoadMetadata(string assemblyFileName, string pdbFileName, bool prefetchMetadata)
        {
            if (string.IsNullOrWhiteSpace(assemblyFileName))
            {
                throw new ArgumentException("You must provide a valid assembly filename", nameof(assemblyFileName));
            }
            if (string.IsNullOrWhiteSpace(pdbFileName))
            {
                throw new ArgumentException("You must provide a valid PDB filename", nameof(pdbFileName));
            }

            var assemblyFile = new FileInfo(assemblyFileName);
            var pdbFile = new FileInfo(pdbFileName);
            return LoadMetadata(assemblyFile, pdbFile, prefetchMetadata);
        }

        public static AssemblyToExpose LoadMetadata(FileInfo assemblyFile, FileInfo pdbFile, bool prefetchMetadata)
        {
            if (assemblyFile == null)
            {
                throw new ArgumentNullException(nameof(assemblyFile));
            }
            if (pdbFile == null)
            {
                throw new ArgumentNullException(nameof(pdbFile));
            }

            return LoadMetadata0(assemblyFile, pdbFile, prefetchMetadata);
        }

        private static AssemblyToExpose LoadMetadata0(FileInfo assemblyFile, FileInfo pdbFile, bool prefetchMetadata)
        {
            if (!assemblyFile.Exists)
            {
                throw new FileNotFoundException("Assembly file not found", assemblyFile.FullName);
            }
            if (!pdbFile?.Exists == true)
            {
                throw new FileNotFoundException("PDB file not found", pdbFile.FullName);
            }

            var metadataState = new MetadataState(prefetchMetadata, assemblyFile, pdbFile);
            if (!metadataState.HasMetadata)
            {
                throw new ArgumentException($"No metadata found in assembly {assemblyFile.FullName}", nameof(assemblyFile));
            }

            return metadataState.AssemblyDefinition;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override string ToString() => $"({GetType().FullName}) {FullName}";

        protected abstract void Dispose(bool disposeManaged);
    }

#if NETSTANDARD2_0 || NET_FRAMEWORK
    public abstract partial class Assembly : AssemblyToExpose, IDisposable
    {
        public abstract AssemblyFlags Flags { get; }

        public abstract AssemblyHashAlgorithm HashAlgorithm { get; }

        public override string FullName => GetName().FullName;

        public override bool GlobalAssemblyCache => throw NotSupportedHelper.NotValidForMetadata();

        public override long HostContext => throw NotSupportedHelper.NotValidForMetadata();

        public override string ImageRuntimeVersion => throw NotSupportedHelper.FutureVersion();

        public override bool ReflectionOnly => true;
    }
#else
    public abstract partial class Assembly : IDisposable
    {
        public abstract string CodeBase { get; }

        public abstract MethodInfoToExpose EntryPoint { get; }

        public abstract string Location { get; }

        public abstract ModuleToExpose ManifestModule { get; }

        public override string ToString() => FullName ?? base.ToString();

        public virtual AssemblyName GetName() => GetName(false);

        public virtual IEnumerable<TypeToExpose> ExportedTypes => GetExportedTypes();

        public virtual string FullName => GetName().FullName;

        public virtual IEnumerable<TypeInfoToExpose> DefinedTypes => GetTypes().Cast<TypeInfo>();

        public virtual IEnumerable<ModuleToExpose> Modules => GetModules(true);

        public virtual bool ReflectionOnly => true;

        public virtual IEnumerable<CustomAttributeDataToExpose> CustomAttributes => GetCustomAttributesData();

        public abstract IList<CustomAttributeDataToExpose> GetCustomAttributesData();

        public abstract TypeToExpose[] GetExportedTypes();

        public abstract ManifestResourceInfo GetManifestResourceInfo(string resourceName);

        public abstract string[] GetManifestResourceNames();

        public abstract ModuleToExpose GetModule(string name);

        public abstract ModuleToExpose[] GetModules(bool getResourceModules);

        public abstract AssemblyName GetName(bool copiedName);

        public abstract AssemblyName[] GetReferencedAssemblies();

        public abstract TypeToExpose[] GetTypes();
    }
#endif
}
