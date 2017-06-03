using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    [PublicAPI]
    public class Document : MetadataDom.Document, IManagedCodeElement
    {
        private readonly Lazy<byte[]> _hash;
        private readonly Lazy<string> _sourceCode;
        private readonly Lazy<string[]> _sourceCodeLines;

        internal Document(DocumentHandle metadataHandle, MetadataState metadataState)
        {
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            Key = new CodeElementKey<Document>(metadataHandle);
            RawMetadata = MetadataState.PdbReader.GetDocument(metadataHandle);
            Name = MetadataState.PdbReader.GetString(RawMetadata.Name);
            _hash = new Lazy<byte[]>(() => MetadataState.PdbReader.GetBlobBytes(RawMetadata.Hash));
            HashAlgorithmGuid = MetadataState.PdbReader.GetGuid(RawMetadata.HashAlgorithm);
            HashAlgorithm = HashAlgorithmGuid.Equals(Guid.Empty) ? (AssemblyHashAlgorithm?) null : KnownHashAlgorithmGuids.MapFromGuid[HashAlgorithmGuid];
            LanguageGuid = MetadataState.PdbReader.GetGuid(RawMetadata.Language);
            Language = KnownLanguageGuids.MapFromGuid[LanguageGuid];
            _sourceCode = new Lazy<string>(() => File.Exists(Name) ? File.ReadAllText(Name) : null);
            _sourceCodeLines = new Lazy<string[]>(() => SourceCode == null ? null : Regex.Split(SourceCode, "\r\n|\r|\n"));
        }

        public string FullName => Name;

        public override byte[] Hash => _hash.Value;

        public override AssemblyHashAlgorithm? HashAlgorithm { get; }

        public override sealed Guid HashAlgorithmGuid { get; }

        public override Language Language { get; }

        public override sealed Guid LanguageGuid { get; }

        public DocumentHandle MetadataHandle { get; }

        public override int MetadataToken => Key.Handle.Value.GetHashCode();

        public override string Name { get; }

        public System.Reflection.Metadata.Document RawMetadata { get; }

        public override string SourceCode => _sourceCode.Value;

        public override string[] SourceCodeLines => _sourceCodeLines.Value;

        public string TextSignature => Name;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
