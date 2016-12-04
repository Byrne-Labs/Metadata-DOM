using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class AssemblyDefinition : CodeElementWithHandle
    {
        private readonly Lazy<string> _culture;
        private readonly Lazy<IReadOnlyList<CustomAttribute>> _customAttributes;
        private readonly Lazy<IReadOnlyList<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
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
            _customAttributes = new Lazy<IReadOnlyList<CustomAttribute>>(() => GetCodeElements<CustomAttribute>(assemblyDefinition.GetCustomAttributes()));
            _declarativeSecurityAttributes = new Lazy<IReadOnlyList<DeclarativeSecurityAttribute>>(() => GetCodeElements<DeclarativeSecurityAttribute>(assemblyDefinition.GetDeclarativeSecurityAttributes()));
        }

        public string Culture => _culture.Value;

        public IReadOnlyList<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public IReadOnlyList<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public AssemblyFlags Flags { get; }

        public AssemblyHashAlgorithm HashAlgorithm { get; }

        public string Name => _name.Value;

        public Blob PublicKey => _publicKey.Value;

        public Version Version { get; }
    }
}
