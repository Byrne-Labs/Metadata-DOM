using System;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace ByrneLabs.Commons.MetadataDom
{
    internal class MetadataFile : IDisposable
    {
        private readonly MetadataReaderProvider _metadataReaderProvider;

        public MetadataFile(bool prefetchMetadata, FileInfo file, MetadataFileType fileType)
        {
            var fileStream = file.OpenRead();

            switch (fileType)
            {
                case MetadataFileType.Assembly:
                    PEReader = new PEReader(fileStream, prefetchMetadata ? PEStreamOptions.PrefetchEntireImage : PEStreamOptions.Default);
                    if (PEReader.HasMetadata)
                    {
                        HasMetadata = PEReader.HasMetadata;
                        Reader = PEReader.GetMetadataReader();
                    }
                    break;
                case MetadataFileType.Pdb:
                    _metadataReaderProvider = MetadataReaderProvider.FromPortablePdbStream(fileStream, prefetchMetadata ? MetadataStreamOptions.PrefetchMetadata : MetadataStreamOptions.Default);
                    Reader = _metadataReaderProvider.GetMetadataReader();
                    HasMetadata = Reader.DebugMetadataHeader != null;
                    break;
                default:
                    throw new ArgumentException($"Invalid file type {fileType}");
            }
        }

        public PEReader PEReader { get; }

        public bool HasMetadata { get; }

        public MetadataReader Reader { get; }

        public void Dispose()
        {
            PEReader?.Dispose();
            _metadataReaderProvider?.Dispose();
        }
    }
}
