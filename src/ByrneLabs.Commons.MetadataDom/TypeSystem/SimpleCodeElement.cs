using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public abstract class SimpleCodeElement : IManagedCodeElement
    {
        internal SimpleCodeElement(CodeElementKey key, MetadataState metadataState)
        {
            Key = key;
            MetadataState = metadataState;
        }

        internal SimpleCodeElement(Handle metadataHandle, MetadataState metadataState) : this(new CodeElementKey(metadataHandle), metadataState)
        {
        }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
