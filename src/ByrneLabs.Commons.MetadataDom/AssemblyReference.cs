using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Name.FullName}")]
    public class AssemblyReference : AssemblyBase<AssemblyReference, AssemblyReferenceHandle, System.Reflection.Metadata.AssemblyReference>
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<Blob> _hashValue;

        internal AssemblyReference(AssemblyReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var name = new AssemblyName
            {
                Name = AsString(RawMetadata.Name),
                CultureName = AsString(RawMetadata.Culture),
                Flags = (RawMetadata.Flags.HasFlag(AssemblyFlags.PublicKey) ? AssemblyNameFlags.PublicKey : 0) | (RawMetadata.Flags.HasFlag(AssemblyFlags.Retargetable) ? AssemblyNameFlags.Retargetable : 0),
                ContentType = RawMetadata.Flags.HasFlag(AssemblyFlags.WindowsRuntime) ? AssemblyContentType.WindowsRuntime : AssemblyContentType.Default,
                Version = RawMetadata.Version
            };
            var publicKeyOrToken = Reader.GetBlobBytes(RawMetadata.PublicKeyOrToken);
            if (publicKeyOrToken.Length == 8)
            {
                name.SetPublicKeyToken(publicKeyOrToken);
            }
            else
            {
                name.SetPublicKey(publicKeyOrToken);
            }
            Name = name;

            _hashValue = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(RawMetadata.HashValue)));
            Flags = RawMetadata.Flags;

            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.GetCustomAttributes" />
        public override ImmutableArray<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.Flags" />
        public override AssemblyFlags Flags { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.HashValue" />
        public Blob HashValue => _hashValue.Value;

        public override AssemblyName Name { get; }
    }
}
