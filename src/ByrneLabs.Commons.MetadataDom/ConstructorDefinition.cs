using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
    public class ConstructorDefinition : MethodDefinitionBase
    {
        internal ConstructorDefinition(MethodDefinitionHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
        }
    }
}
