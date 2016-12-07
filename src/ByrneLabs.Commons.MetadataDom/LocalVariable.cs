using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.LocalVariable" />
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

        /// <inheritdoc cref="System.Reflection.Metadata.LocalVariable.Attributes" />
        public LocalVariableAttributes Attributes { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.LocalVariable.Index" />
        public int Index { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.LocalVariable.Name" />
        public string Name => _name.Value;
    }
}
