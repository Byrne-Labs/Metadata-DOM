using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class LocalVariable : DebugCodeElement, ICodeElementWithTypedHandle<LocalVariableHandle, System.Reflection.Metadata.LocalVariable>
    {
        internal LocalVariable(LocalVariableHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetLocalVariable(metadataHandle);
            Name = AsString(RawMetadata.Name);
            Attributes = RawMetadata.Attributes;
            Index = RawMetadata.Index;
        }
        public LocalVariableAttributes Attributes { get; }
        public int Index { get; }
        public string Name { get; }

        public System.Reflection.Metadata.LocalVariable RawMetadata { get; }

        public LocalVariableHandle MetadataHandle { get; }
    }
}
