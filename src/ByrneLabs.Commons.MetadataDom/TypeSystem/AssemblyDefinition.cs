using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;
using TypeToExpose = System.Type;
using MethodInfoToExpose = System.Reflection.MethodInfo;
using ModuleToExpose = System.Reflection.Module;

#else
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using MethodInfoToExpose = ByrneLabs.Commons.MetadataDom.MethodInfo;
using ModuleToExpose = ByrneLabs.Commons.MetadataDom.Module;

#endif

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {GetName().FullName}")]
    public class AssemblyDefinition : AssemblyBase<AssemblyDefinition, AssemblyDefinitionHandle, System.Reflection.Metadata.AssemblyDefinition>
    {
        private readonly Lazy<ImmutableArray<AssemblyReference>> _assemblyReferences;
        private readonly Lazy<ImmutableArray<CustomAttribute>> _customAttributes;
        private readonly Lazy<ImmutableArray<DeclarativeSecurityAttribute>> _declarativeSecurityAttributes;
        private readonly Lazy<MethodInfoToExpose> _entryPoint;
        private readonly Lazy<Module> _moduleDefinition;
        private readonly AssemblyName _name;

        internal AssemblyDefinition(AssemblyDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var name = new AssemblyName
            {
                Name = MetadataState.AssemblyReader.GetString(RawMetadata.Name),
                CultureName = MetadataState.AssemblyReader.GetString(RawMetadata.Culture) ?? string.Empty,
                Flags = (RawMetadata.Flags.HasFlag(AssemblyFlags.PublicKey) ? AssemblyNameFlags.PublicKey : 0) | (RawMetadata.Flags.HasFlag(AssemblyFlags.Retargetable) ? AssemblyNameFlags.Retargetable : 0),
                ContentType = RawMetadata.Flags.HasFlag(AssemblyFlags.WindowsRuntime) ? AssemblyContentType.WindowsRuntime : AssemblyContentType.Default,
                Version = RawMetadata.Version
            };
            name.SetPublicKey(MetadataState.AssemblyReader.GetBlobBytes(RawMetadata.PublicKey));
            _name = name;

            _entryPoint = new Lazy<MethodInfoToExpose>(() => MetadataState.HasDebugMetadata ? MetadataState.GetCodeElement<MethodDefinition>(MetadataState.PdbReader.DebugMetadataHeader.EntryPoint) : null);
            _assemblyReferences = MetadataState.GetLazyCodeElements<AssemblyReference>(MetadataState.AssemblyReader.AssemblyReferences);
            Flags = RawMetadata.Flags;
            HashAlgorithm = RawMetadata.HashAlgorithm;
            _customAttributes = MetadataState.GetLazyCodeElements<CustomAttribute>(RawMetadata.GetCustomAttributes());
            _declarativeSecurityAttributes = MetadataState.GetLazyCodeElements<DeclarativeSecurityAttribute>(RawMetadata.GetDeclarativeSecurityAttributes());
            _moduleDefinition = MetadataState.GetLazyCodeElement<Module>(Handle.ModuleDefinition);
        }

        public override string CodeBase => throw new NotSupportedException("This will be supported in the future");

        public IEnumerable<DeclarativeSecurityAttribute> DeclarativeSecurityAttributes => _declarativeSecurityAttributes.Value;

        public override MethodInfoToExpose EntryPoint => _entryPoint.Value;

        public override string EscapedCodeBase => throw new NotSupportedException("This will be supported in the future");

        public override AssemblyFlags Flags { get; }

        public override AssemblyHashAlgorithm HashAlgorithm { get; }

        public override string Location => MetadataState.AssemblyFileWrapper.CompiledFile.FullName;

        public override ModuleToExpose ManifestModule => Modules.OfType<Module>().SingleOrDefault(module => module.Manifest);

        public override IList<CustomAttributeDataToExpose> GetCustomAttributesData() => _customAttributes.Value.ToImmutableList< CustomAttributeDataToExpose>();

        public override TypeToExpose[] GetExportedTypes() => MetadataState.DefinedTypes.Where(type => type.IsPublic).Cast<Type>().ToArray();

        public override ModuleToExpose[] GetLoadedModules(bool getResourceModules) => throw new NotSupportedException("This will be supported in the future");

        public override ManifestResourceInfo GetManifestResourceInfo(string resourceName) => throw new NotSupportedException("This will be supported in the future");

        public override string[] GetManifestResourceNames() => throw new NotSupportedException("This will be supported in the future");

        [SuppressMessage("ReSharper", "RedundantEnumerableCastCall", Justification = "Needed for .NET Core prior to 2.0")]
        public override ModuleToExpose GetModule(string name) => Modules.OfType<ModuleToExpose>().SingleOrDefault(module => module.Name.Equals(name));

        public override ModuleToExpose[] GetModules(bool getResourceModules) => new List<ModuleToExpose> { _moduleDefinition.Value }.ToArray();

        public override AssemblyName GetName(bool copiedName) => _name;

        public override AssemblyName[] GetReferencedAssemblies() => _assemblyReferences.Value.Select(assemblyReference => assemblyReference.GetName()).ToArray();

        public override TypeToExpose[] GetTypes() => MetadataState.DefinedTypes.Cast<TypeToExpose>().ToArray();
    }
}
