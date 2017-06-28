using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {GetName().FullName}")]
    public class AssemblyDefinition : AssemblyBase<AssemblyDefinition, AssemblyDefinitionHandle, System.Reflection.Metadata.AssemblyDefinition>
    {
        private readonly Lazy<IEnumerable<AssemblyReference>> _assemblyReferences;
        private readonly Lazy<IEnumerable<CustomAttribute>> _customAttributes;
        private readonly Lazy<IEnumerable<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<MethodInfo> _entryPoint;
        private readonly Lazy<Module> _moduleDefinition;
        private readonly AssemblyName _name;

        internal AssemblyDefinition(AssemblyDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var name = new AssemblyName
            {
                Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name),
                CultureName = MetadataState.AssemblyReader.GetString(RawMetadata.Culture) ?? string.Empty,
                Flags = ((RawMetadata.Flags & AssemblyFlags.PublicKey) != 0 ? AssemblyNameFlags.PublicKey : 0) | ((RawMetadata.Flags & AssemblyFlags.Retargetable) != 0 ? AssemblyNameFlags.Retargetable : 0),
                ContentType = (RawMetadata.Flags & AssemblyFlags.WindowsRuntime) != 0 ? AssemblyContentType.WindowsRuntime : AssemblyContentType.Default,
                Version = RawMetadata.Version
            };
            name.SetPublicKey(MetadataState.AssemblyReader.GetBlobBytes(RawMetadata.PublicKey));
            _name = name;

            _entryPoint = new Lazy<MethodInfo>(() => MetadataState.HasDebugMetadata ? MetadataState.GetCodeElement<MethodDefinition>(MetadataState.PdbReader.DebugMetadataHeader.EntryPoint) : null);
            _assemblyReferences = MetadataState.GetLazyCodeElements<AssemblyReference>(MetadataState.AssemblyReader.AssemblyReferences);
            Flags = RawMetadata.Flags;
            HashAlgorithm = RawMetadata.HashAlgorithm;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(RawMetadata.GetDeclarativeSecurityAttributes());
            _moduleDefinition = MetadataState.GetLazyCodeElement<Module>(Handle.ModuleDefinition);
        }

        public override string CodeBase => throw NotSupportedHelper.FutureVersion();

        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public override System.Reflection.MethodInfo EntryPoint => _entryPoint.Value;

        public override string EscapedCodeBase => throw NotSupportedHelper.FutureVersion();

        public override AssemblyFlags Flags { get; }

        public override AssemblyHashAlgorithm HashAlgorithm { get; }

        public override string Location => MetadataState.AssemblyFileWrapper.CompiledFile.FullName;

        public override System.Reflection.Module ManifestModule => Modules.OfType<Module>().SingleOrDefault(module => module.Manifest);

        public override IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList<System.Reflection.CustomAttributeData>();

        public override Type[] GetExportedTypes() => MetadataState.DefinedTypes.Where(type => type.IsPublic || type.IsNestedPublic).Cast<Type>().ToArray();

        public override System.Reflection.Module[] GetLoadedModules(bool getResourceModules) => throw NotSupportedHelper.FutureVersion();

        public override ManifestResourceInfo GetManifestResourceInfo(string resourceName) => throw NotSupportedHelper.FutureVersion();

        public override string[] GetManifestResourceNames() => throw NotSupportedHelper.FutureVersion();

        [SuppressMessage("ReSharper", "RedundantEnumerableCastCall", Justification = "Needed for .NET Core prior to 2.0")]
        public override System.Reflection.Module GetModule(string name) => Modules.OfType<Module>().SingleOrDefault(module => module.Name.Equals(name));

        public override System.Reflection.Module[] GetModules(bool getResourceModules) => new List<System.Reflection.Module> { _moduleDefinition.Value }.ToArray();

        public override AssemblyName GetName(bool copiedName) => _name;

        public override AssemblyName[] GetReferencedAssemblies() => _assemblyReferences.Value.Select(assemblyReference => assemblyReference.GetName()).ToArray();

        public override Type[] GetTypes() => MetadataState.DefinedTypes.Cast<Type>().ToArray();
    }
}
