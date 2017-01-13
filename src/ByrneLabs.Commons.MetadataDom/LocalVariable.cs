using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.LocalVariable" />
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

        /// <inheritdoc cref="System.Reflection.Metadata.LocalVariable.Attributes" />
        public LocalVariableAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.LocalVariable.Index" />
        public int Index { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.LocalVariable.Name" />
        public string Name { get; }

        public System.Reflection.Metadata.LocalVariable RawMetadata { get; }

        public LocalVariableHandle MetadataHandle { get; }
    }
}
