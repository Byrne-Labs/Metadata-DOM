using System;
using System.Collections.Generic;
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
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<Blob> _hashValue;

        internal AssemblyReference(AssemblyReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var name = new AssemblyName
            {
                Name = AsString(MetadataToken.Name),
                CultureName = AsString(MetadataToken.Culture),
                Flags = (MetadataToken.Flags.HasFlag(AssemblyFlags.PublicKey) ? AssemblyNameFlags.PublicKey : 0) | (MetadataToken.Flags.HasFlag(AssemblyFlags.Retargetable) ? AssemblyNameFlags.Retargetable : 0),
                ContentType = MetadataToken.Flags.HasFlag(AssemblyFlags.WindowsRuntime) ? AssemblyContentType.WindowsRuntime : AssemblyContentType.Default,
                Version = MetadataToken.Version
            };
            var publicKeyOrToken = Reader.GetBlobBytes(MetadataToken.PublicKeyOrToken);
            if (publicKeyOrToken.Length == 8)
            {
                name.SetPublicKeyToken(publicKeyOrToken);
            }
            else
            {
                name.SetPublicKey(publicKeyOrToken);
            }
            Name = name;

            _hashValue = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.HashValue)));
            Flags = MetadataToken.Flags;

            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(MetadataToken.GetCustomAttributes());
        }

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.GetCustomAttributes" />
        public override IEnumerable<CustomAttribute> CustomAttributes => _customAttributes.Value;

        public override IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes { get; } = new List<DeclarativeSecurityAttribute>();

        public override IEnumerable<TypeBase> DefinedTypes { get; } = new List<TypeBase>();

        public override IMethod EntryPoint { get; } = null;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.Flags" />
        public override AssemblyFlags Flags { get; }

        public override AssemblyHashAlgorithm HashAlgorithm { get; } = AssemblyHashAlgorithm.None;

        /// <inheritdoc cref="System.Reflection.Metadata.AssemblyReference.HashValue" />
        public Blob HashValue => _hashValue.Value;

        public override AssemblyName Name { get; }

        public override IEnumerable<IAssembly> ReferencedAssemblies { get; } = new List<IAssembly>();
    }
}
