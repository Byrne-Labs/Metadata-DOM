using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name}")]
    public class AssemblyFile : SimpleCodeElement
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<Blob> _hashValue;

        internal AssemblyFile(AssemblyFileHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.AssemblyReader.GetAssemblyFile(metadataHandle);
            Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name);
            _hashValue = MetadataState.GetLazyCodeElement<Blob>(RawMetadata.HashValue, false);
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            ContainsMetadata = RawMetadata.ContainsMetadata;
        }

        public bool ContainsMetadata { get; }

        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public Blob HashValue => _hashValue.Value;

        public AssemblyFileHandle MetadataHandle { get; }

        public string Name { get; }

        public System.Reflection.Metadata.AssemblyFile RawMetadata { get; }
    }
}
