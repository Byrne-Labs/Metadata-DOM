using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition" />
    [PublicAPI]
    public class AssemblyDefinition : CodeElementWithHandle
    {
        private readonly Lazy<string> _culture;
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<string> _name;
        private readonly Lazy<Blob> _publicKey;

        internal AssemblyDefinition(AssemblyDefinitionHandle assemblyDefinitionHandle, MetadataState metadataState) : base(assemblyDefinitionHandle, metadataState)
        {
            var assemblyDefinition = Reader.GetAssemblyDefinition();
            _name = new Lazy<string>(() => AsString(assemblyDefinition.Name));
            _culture = new Lazy<string>(() => AsString(assemblyDefinition.Culture));
            Flags = assemblyDefinition.Flags;
            HashAlgorithm = assemblyDefinition.HashAlgorithm;
            _publicKey = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(assemblyDefinition.PublicKey)));
            Version = assemblyDefinition.Version;
            _customAttributes = new Lazy<IEnumerable<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(assemblyDefinition.GetCustomAttributes()));
            _declarativeSecurityAttributes = new Lazy<IEnumerable<DeclarativeSecurityAttribute>>(() => GetCodeElements<DeclarativeSecurityAttribute>(assemblyDefinition.GetDeclarativeSecurityAttributes()));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.Culture" />
        public string Culture => _culture.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.GetCustomAttributes" />
        public IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.GetDeclarativeSecurityAttributes" />
        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.Flags" />
        public AssemblyFlags Flags { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.HashAlgorithm" />
        public AssemblyHashAlgorithm HashAlgorithm { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.Name" />
        public string Name => _name.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.PublicKey" />
        public Blob PublicKey => _publicKey.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyDefinition.Version" />
        public Version Version { get; }
    }
}
