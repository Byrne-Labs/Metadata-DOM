using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace ByrneLabs.Commons.MetadataDom
{
    internal class CompiledFileWrapper : IDisposable
    {
        private readonly MetadataReaderProvider _metadataReaderProvider;

        public CompiledFileWrapper(bool prefetchMetadata, FileInfo assemblyFile)
        {
            File = assemblyFile;
            var fileStream = assemblyFile.OpenRead();

            PEReader = new PEReader(fileStream, prefetchMetadata ? PEStreamOptions.PrefetchEntireImage : PEStreamOptions.Default);
            if (PEReader.HasMetadata)
            {
                HasMetadata = PEReader.HasMetadata;
                Reader = PEReader.GetMetadataReader();
            }
        }

        public CompiledFileWrapper(bool prefetchMetadata, CompiledFileWrapper assemblyFileWrapper, FileInfo pdbFile)
        {
            var debugDirectoryEntries = assemblyFileWrapper?.PEReader.PEHeaders.IsCoffOnly != false ? null : assemblyFileWrapper?.PEReader.ReadDebugDirectory();
            var pdbDebugEntries = debugDirectoryEntries?.Where(debugDirectoryEntry => debugDirectoryEntry.Type == DebugDirectoryEntryType.EmbeddedPortablePdb);

            if (pdbDebugEntries?.Any() == true)
            {
                _metadataReaderProvider = assemblyFileWrapper.PEReader.ReadEmbeddedPortablePdbDebugDirectoryData(pdbDebugEntries.First());
            }
            else
            {
                if (!pdbFile?.Exists == true)
                {
                    throw new ArgumentException($"The file {pdbFile.FullName} does not exist", nameof(pdbFile));
                }
                var codeViewDebugEntries = debugDirectoryEntries?.Where(debugDirectoryEntry => debugDirectoryEntry.Type == DebugDirectoryEntryType.CodeView);
                if (codeViewDebugEntries?.Any() == true)
                {
                    var debugData = assemblyFileWrapper.PEReader.ReadCodeViewDebugDirectoryData(codeViewDebugEntries.First());
                    pdbFile = new FileInfo(debugData.Path);
                }
                if (pdbFile?.Exists == false)
                {
                    pdbFile = new FileInfo(assemblyFileWrapper.File.FullName.Substring(0, assemblyFileWrapper.File.FullName.Length - assemblyFileWrapper.File.Extension.Length) + ".pdb");
                }
                if (pdbFile?.Exists == true)
                {
                    var fileStream = pdbFile.OpenRead();
                    _metadataReaderProvider = MetadataReaderProvider.FromPortablePdbStream(fileStream, prefetchMetadata ? MetadataStreamOptions.PrefetchMetadata : MetadataStreamOptions.Default);
                }
            }

            if (_metadataReaderProvider != null)
            {
                Reader = _metadataReaderProvider.GetMetadataReader();
                HasMetadata = Reader.DebugMetadataHeader != null;
            }
        }

        public FileInfo File { get; }

        public bool HasMetadata { get; }

        public PEReader PEReader { get; }

        public MetadataReader Reader { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool managedOnly)
        {
            if (managedOnly)
            {
                PEReader?.Dispose();
                _metadataReaderProvider?.Dispose();
            }
        }
    }
}
