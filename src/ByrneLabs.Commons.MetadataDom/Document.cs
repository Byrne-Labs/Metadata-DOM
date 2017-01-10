using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.Document" />
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    //[PublicAPI]
    public class Document : DebugCodeElement, ICodeElementWithHandle<DocumentHandle, System.Reflection.Metadata.Document>
    {
        private static readonly IDictionary<Guid, HashAlgorithm> KnownHashAlgorithms = new Dictionary<Guid, HashAlgorithm>
        {
            { Guid.Parse("ff1816ec-aa5e-4d10-87f7-6f4963833460"), MetadataDom.HashAlgorithm.Sha1 },
            { Guid.Parse("8829d00f-11b8-4213-878b-770e8597ac16"), MetadataDom.HashAlgorithm.Sha256 }
        };
        private static readonly IDictionary<Guid, Language> KnownLanguages = new Dictionary<Guid, Language>
        {
            { new Guid(0x63a08714, unchecked((short) 0xfc37), 0x11d2, 0x90, 0x4c, 0x0, 0xc0, 0x4f, 0xa3, 0x02, 0xa1), Language.C },
            { new Guid(0x3a12d0b7, unchecked((short) 0xc26c), 0x11d0, 0xb4, 0x42, 0x0, 0xa0, 0x24, 0x4a, 0x1d, 0xd2), Language.CPlusPlus },
            { new Guid(0x3f5162f8, 0x07c6, 0x11d3, 0x90, 0x53, 0x0, 0xc0, 0x4f, 0xa3, 0x02, 0xa1), Language.CSharp },
            { new Guid(0x3a12d0b8, unchecked((short) 0xc26c), 0x11d0, 0xb4, 0x42, 0x0, 0xa0, 0x24, 0x4a, 0x1d, 0xd2), Language.VisualBasic },
            { new Guid(0x3a12d0b4, unchecked((short) 0xc26c), 0x11d0, 0xb4, 0x42, 0x0, 0xa0, 0x24, 0x4a, 0x1d, 0xd2), Language.Java },
            { new Guid(unchecked((int) 0xaf046cd1), unchecked((short) 0xd0e1), 0x11d2, 0x97, 0x7c, 0x0, 0xa0, 0xc9, 0xb4, 0xd5, 0xc), Language.Cobol },
            { new Guid(unchecked((int) 0xaf046cd2), unchecked((short) 0xd0e1), 0x11d2, 0x97, 0x7c, 0x0, 0xa0, 0xc9, 0xb4, 0xd5, 0xc), Language.Pascal },
            { new Guid(unchecked((int) 0xaf046cd3), unchecked((short) 0xd0e1), 0x11d2, 0x97, 0x7c, 0x0, 0xa0, 0xc9, 0xb4, 0xd5, 0xc), Language.ILAssembly },
            { new Guid(0x3a12d0b6, unchecked((short) 0xc26c), 0x11d0, 0xb4, 0x42, 0x00, 0xa0, 0x24, 0x4a, 0x1d, 0xd2), Language.JScript },
            { new Guid(0xd9b9f7b, 0x6611, 0x11d3, 0xbd, 0x2a, 0x0, 0x0, 0xf8, 0x8, 0x49, 0xbd), Language.SMC },
            { new Guid(0x4b35fde8, 0x07c6, 0x11d3, 0x90, 0x53, 0x0, 0xc0, 0x4f, 0xa3, 0x02, 0xa1), Language.MCPlusPlus },
            { new Guid(unchecked((int) 0xab4f38c9), unchecked((short) 0xb6e6), 0x43ba, 0xbe, 0x3b, 0x58, 0x08, 0x0b, 0x2c, 0xcc, 0xe3), Language.FSharp }
        };
        private readonly Lazy<Blob> _hash;
        private readonly Lazy<string> _sourceCode;
        private readonly Lazy<string[]> _sourceCodeLines;

        internal Document(DocumentHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetDocument(metadataHandle);
            Name = Reader.GetString(MetadataToken.Name);
            _hash = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.Hash)));
            HashAlgorithmGuid = AsGuid(MetadataToken.HashAlgorithm);
            HashAlgorithm = HashAlgorithmGuid.Equals(Guid.Empty) ? (HashAlgorithm?) null : KnownHashAlgorithms[HashAlgorithmGuid];
            LanguageGuid = AsGuid(MetadataToken.Language);
            Language = KnownLanguages[LanguageGuid];
            _sourceCode = new Lazy<string>(() => File.Exists(Name) ? File.ReadAllText(Name) : null);
            _sourceCodeLines = new Lazy<string[]>(() => SourceCode == null ? null : Regex.Split(SourceCode, "\r\n|\r|\n"));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.Document.Hash" />
        /// <remarks>
        ///     <see cref="Document.HashAlgorithm" /> determines the algorithm used to produce this hash. The source MetadataToken is hashed in its binary form as stored in the file.</remarks>
        public Blob Hash => _hash.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.Document.HashAlgorithm" />
        /// <summary>Hash algorithm used to calculate <see cref="Document.Hash" /> (SHA1, SHA256, etc.)</summary>
        public HashAlgorithm? HashAlgorithm { get; }

        public Guid HashAlgorithmGuid { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.Document.Language" />
        public Language Language { get; }

        public Guid LanguageGuid { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.Document.Name" />
        public string Name { get; }

        public string SourceCode => _sourceCode.Value;

        internal string[] SourceCodeLines => _sourceCodeLines.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public DocumentHandle MetadataHandle { get; }

        public System.Reflection.Metadata.Document MetadataToken { get; }
    }
}
