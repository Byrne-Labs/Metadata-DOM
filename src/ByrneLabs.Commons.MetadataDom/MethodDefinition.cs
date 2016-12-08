using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class MethodDefinition : MethodDefinitionBase
    {
        internal MethodDefinition(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
        }
    }
}
