﻿using System;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    public abstract class Document
    {
        public abstract byte[] Hash { get; }

        public abstract AssemblyHashAlgorithm? HashAlgorithm { get; }

        public abstract Guid HashAlgorithmGuid { get; }

        public abstract Language Language { get; }

        public abstract Guid LanguageGuid { get; }

        public abstract int MetadataToken { get; }

        public abstract string Name { get; }

        public abstract string SourceCode { get; }

        public abstract string[] SourceCodeLines { get; }

        public string GetSourceCode(int startRow, int startColumn, int endRow, int endColumn) => throw new NotImplementedException();
    }
}
