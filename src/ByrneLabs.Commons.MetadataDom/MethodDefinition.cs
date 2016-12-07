using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class MethodDefinition : MethodDefinitionBase
    {
        internal MethodDefinition(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
        }
    }
}
