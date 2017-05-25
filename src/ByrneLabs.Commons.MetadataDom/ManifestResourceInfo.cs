#if !(NETSTANDARD2_0 || NET_FRAMEWORK)
using System.Reflection;
using JetBrains.Annotations;
using AssemblyToExpose = ByrneLabs.Commons.MetadataDom.Assembly;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class ManifestResourceInfo
    {
        public ManifestResourceInfo(AssemblyToExpose containingAssembly, string containingFileName, ResourceLocation resourceLocation)
        {
            ReferencedAssembly = containingAssembly;
            FileName = containingFileName;
            ResourceLocation = resourceLocation;
        }

        public string FileName { get; }

        public AssemblyToExpose ReferencedAssembly { get; }

        public ResourceLocation ResourceLocation { get; }
    }
}
#endif
