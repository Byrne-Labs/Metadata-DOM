using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    public class LocalVariable : DebugCodeElementWithHandle
    {
        private readonly Lazy<string> _name;

        internal LocalVariable(LocalVariableHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var localVariable = Reader.GetLocalVariable(metadataHandle);
            _name = new Lazy<string>(() => AsString(localVariable.Name));
            Attributes = localVariable.Attributes;
            Index = localVariable.Index;
        }

        public LocalVariableAttributes Attributes { get; }

        public int Index { get; }

        public string Name => _name.Value;
    }
}
