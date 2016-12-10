using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition" />
    [PublicAPI]
    public class AssemblyDefinition : RuntimeCodeElement, ICodeElementWithHandle<AssemblyDefinitionHandle, System.Reflection.Metadata.AssemblyDefinition>
    {
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<Blob> _publicKey;

        internal AssemblyDefinition(AssemblyDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetAssemblyDefinition();
            Name = AsString(MetadataToken.Name);
            Culture = AsString(MetadataToken.Culture);
            Flags = MetadataToken.Flags;
            HashAlgorithm = MetadataToken.HashAlgorithm;
            _publicKey = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.PublicKey)));
            Version = MetadataToken.Version;
            _customAttributes = GetLazyCodeElementsWithHandle<CustomAttribute>(MetadataToken.GetCustomAttributes());
            _declarativeSecurityAttributes = GetLazyCodeElementsWithHandle<DeclarativeSecurityAttribute>(MetadataToken.GetDeclarativeSecurityAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.Culture" />
        public string Culture { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.GetDeclarativeSecurityAttributes" />
        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.Flags" />
        public AssemblyFlags Flags { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.HashAlgorithm" />
        public AssemblyHashAlgorithm HashAlgorithm { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.Name" />
        public string Name { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.PublicKey" />
        public Blob PublicKey => _publicKey.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.Version" />
        public Version Version { get; }

        public Handle DowncastMetadataHandle => MetadataHandle;

        public AssemblyDefinitionHandle MetadataHandle { get; }

        public System.Reflection.Metadata.AssemblyDefinition MetadataToken { get; }
    }
}
