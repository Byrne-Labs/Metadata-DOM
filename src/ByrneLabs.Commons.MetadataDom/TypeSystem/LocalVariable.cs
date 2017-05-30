using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    //[PublicAPI]
    public class LocalVariable : LocalVariableInfo, IManagedCodeElement
    {
        internal LocalVariable(LocalVariableHandle metadataHandle, MetadataState metadataState)
        {
            Key = new CodeElementKey<LocalConstant>(metadataHandle);
            MetadataState = metadataState;
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.PdbReader.GetLocalVariable(MetadataHandle);
            Name = MetadataState.PdbReader.GetString(RawMetadata.Name);
            Attributes = RawMetadata.Attributes;
            Index = RawMetadata.Index;
        }

        public LocalVariableAttributes Attributes { get; }

        public override int Index { get; }

        public override bool IsDebuggerHidden => throw new NotSupportedException();

        public LocalVariableHandle MetadataHandle { get; }

        public override string Name { get; }

        public System.Reflection.Metadata.LocalVariable RawMetadata { get; }

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
