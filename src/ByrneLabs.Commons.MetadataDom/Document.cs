using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    [DebuggerDisplay("{Name}")]
    public class Document : DebugCodeElementWithHandle, IContainsSourceCode
    {
        private static readonly IDictionary<Guid, HashAlgorithms> KnownHashAlgorithms = new Dictionary<Guid, HashAlgorithms>
        {
            { Guid.Parse("ff1816ec-aa5e-4d10-87f7-6f4963833460"), HashAlgorithms.Sha1 },
            { Guid.Parse("8829d00f-11b8-4213-878b-770e8597ac16"), HashAlgorithms.Sha256 }
        };
        private static readonly IDictionary<Guid, Languages> KnownLanguages = new Dictionary<Guid, Languages>
        {
            { Guid.Parse("3f5162f8-07c6-11d3-9053-00c04fa302a1"), Languages.CSharp },
            { Guid.Parse("ab4f38c9-b6e6-43ba-be3b-58080b2ccce3"), Languages.FSharp },
            { Guid.Parse("3a12d0b8-c26c-11d0-b442-00a0244a1dd2"), Languages.VisualBasic }
        };
        private readonly Lazy<Blob> _hash;
        private readonly Lazy<string> _sourceCode;
        private readonly Lazy<string[]> _sourceCodeLines;

        internal Document(DocumentHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var document = Reader.GetDocument(metadataHandle);
            Name = Reader.GetString(document.Name);
            _hash = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(document.Hash)));
            HashAlgorithmGuid = AsGuid(document.HashAlgorithm);
            HashAlgorithm = KnownHashAlgorithms[HashAlgorithmGuid];
            LanguageGuid = AsGuid(document.Language);
            Language = KnownLanguages[LanguageGuid];
            _sourceCode = new Lazy<string>(() => File.Exists(Name) ? File.ReadAllText(Name) : null);
            _sourceCodeLines = new Lazy<string[]>(() => File.Exists(Name) ? File.ReadAllLines(Name) : null);
        }

        public Blob Hash => _hash.Value;

        public HashAlgorithms HashAlgorithm { get; }

        public Guid HashAlgorithmGuid { get; }

        public Languages Language { get; }

        public Guid LanguageGuid { get; }

        public string Name { get; }

        internal string[] SourceCodeLines => _sourceCodeLines.Value;

        public string SourceCode => _sourceCode.Value;

        public string SourceFile => Name;
    }
}
