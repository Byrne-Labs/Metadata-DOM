using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {GetName().FullName}")]
    public class AssemblyReference : AssemblyBase<AssemblyReference, AssemblyReferenceHandle, System.Reflection.Metadata.AssemblyReference>
    {
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<byte[]> _hashValue;
        private readonly AssemblyName _name;

        internal AssemblyReference(AssemblyReferenceHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            _name = new AssemblyName
            {
                Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name),
                CultureName = MetadataState.AssemblyReader.GetString(RawMetadata.Culture),
                Flags = (RawMetadata.Flags.HasFlag(AssemblyFlags.PublicKey) ? AssemblyNameFlags.PublicKey : 0) | (RawMetadata.Flags.HasFlag(AssemblyFlags.Retargetable) ? AssemblyNameFlags.Retargetable : 0),
                ContentType = RawMetadata.Flags.HasFlag(AssemblyFlags.WindowsRuntime) ? AssemblyContentType.WindowsRuntime : AssemblyContentType.Default,
                Version = RawMetadata.Version
            };
            var publicKeyOrToken = MetadataState.AssemblyReader.GetBlobBytes(RawMetadata.PublicKeyOrToken);
            if (publicKeyOrToken.Length == 8)
            {
                _name.SetPublicKeyToken(publicKeyOrToken);
            }
            else
            {
                _name.SetPublicKey(publicKeyOrToken);
            }

            _hashValue = new Lazy<byte[]>(() => MetadataState.AssemblyReader.GetBlobBytes(RawMetadata.HashValue));
            Flags = RawMetadata.Flags;

            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
        }

        public override string CodeBase => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override IEnumerable<System.Reflection.TypeInfo> DefinedTypes { get; } = ImmutableArray<TypeInfo>.Empty;

        public override System.Reflection.MethodInfo EntryPoint => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override string EscapedCodeBase => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override AssemblyFlags Flags { get; }

        public override AssemblyHashAlgorithm HashAlgorithm { get; } = AssemblyHashAlgorithm.None;

        public byte[] HashValue => _hashValue.Value;

        public override string Location { get; }

        public override System.Reflection.Module ManifestModule { get; }

        public override IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<System.Reflection.CustomAttributeData>();

        public override Type[] GetExportedTypes() => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override System.Reflection.Module[] GetLoadedModules(bool getResourceModules) => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override ManifestResourceInfo GetManifestResourceInfo(string resourceName) => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override string[] GetManifestResourceNames() => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override System.Reflection.Module GetModule(string name) => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override System.Reflection.Module[] GetModules(bool getResourceModules) => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override AssemblyName GetName(bool copiedName) => _name;

        public override AssemblyName[] GetReferencedAssemblies() => throw NotSupportedHelper.NotValidForMetadataType(GetType());

        public override Type[] GetTypes() => throw NotSupportedHelper.NotValidForMetadataType(GetType());
    }
}
