using System;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace ByrneLabs.Commons.MetadataDom
{
    internal class MetadataFile : IDisposable
    {
        private readonly MetadataReaderProvider _metadataReaderProvider;
        private readonly PEReader _peReader;

        public MetadataFile(bool prefetchMetadata, FileInfo file, MetadataFileType fileType)
        {
            var fileStream = file.OpenRead();

            switch (fileType)
            {
                case MetadataFileType.Assembly:
                    _peReader = new PEReader(fileStream, prefetchMetadata ? PEStreamOptions.PrefetchEntireImage : PEStreamOptions.Default);
                    if (_peReader.HasMetadata)
                    {
                        HasMetadata = _peReader.HasMetadata;
                        Reader = _peReader.GetMetadataReader();
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

        public bool HasMetadata { get; }

        public MetadataReader Reader { get; }

        public void Dispose()
        {
            _peReader?.Dispose();
            _metadataReaderProvider?.Dispose();
        }
    }
}
