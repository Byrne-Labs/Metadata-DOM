namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    internal interface IManagedCodeElement
    {
        CodeElementKey Key { get; }

        MetadataState MetadataState { get; }
    }
}
