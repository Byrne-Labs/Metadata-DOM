using System;
using System.IO;
using System.Reflection;
using ByrneLabs.Commons.MetadataDom.TypeSystem;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public abstract class Assembly : System.Reflection.Assembly, IDisposable
    {
        public abstract AssemblyFlags Flags { get; }

        public abstract AssemblyHashAlgorithm HashAlgorithm { get; }

        public override string FullName => GetName().FullName;

        public override bool GlobalAssemblyCache => throw NotSupportedHelper.NotValidForMetadata();

        public override long HostContext => throw NotSupportedHelper.NotValidForMetadata();

        public override string ImageRuntimeVersion => throw NotSupportedHelper.FutureVersion();

        public override bool ReflectionOnly => true;

        public static Assembly LoadMetadata(FileInfo assemblyFile) => LoadMetadata(assemblyFile, null, true);

        public static Assembly LoadMetadata(FileInfo assemblyFile, FileInfo pdbFile) => LoadMetadata(assemblyFile, pdbFile, true);

        public static Assembly LoadMetadata(string assemblyFileName) => LoadMetadata(assemblyFileName, null, true);

        public static Assembly LoadMetadata(string assemblyFileName, string pdbFileName) => LoadMetadata(assemblyFileName, pdbFileName, true);

        public static Assembly LoadMetadata(string assemblyFileName, string pdbFileName, bool prefetchMetadata)
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

        public static Assembly LoadMetadata(FileInfo assemblyFile, FileInfo pdbFile, bool prefetchMetadata)
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

        private static Assembly LoadMetadata0(FileInfo assemblyFile, FileInfo pdbFile, bool prefetchMetadata)
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

        public override string ToString() => FullName;

        protected abstract void Dispose(bool disposeManaged);
    }
}
