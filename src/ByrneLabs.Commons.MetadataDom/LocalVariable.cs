using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.LocalVariable" />
    [PublicAPI]
    public class LocalVariable : DebugCodeElement, ICodeElementWithHandle<LocalVariableHandle, System.Reflection.Metadata.LocalVariable>
    {
        internal LocalVariable(LocalVariableHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetLocalVariable(metadataHandle);
            Name = AsString(MetadataToken.Name);
            Attributes = MetadataToken.Attributes;
            Index = MetadataToken.Index;
        }

        /// <inheritdoc cref="System.Reflection.Metadata.LocalVariable.Attributes" />
        public LocalVariableAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.LocalVariable.Index" />
        public int Index { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.LocalVariable.Name" />
        public string Name { get; }

        public Handle DowncastMetadataHandle => MetadataHandle;

        public LocalVariableHandle MetadataHandle { get; }

        public System.Reflection.Metadata.LocalVariable MetadataToken { get; }
    }
}
