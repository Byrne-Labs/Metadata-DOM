using System;
using System.Reflection.Metadata;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.Constant" />
    [PublicAPI]
    public class Constant : RuntimeCodeElement, ICodeElementWithHandle<ConstantHandle, System.Reflection.Metadata.Constant>
    {
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<Blob> _value;

        internal Constant(ConstantHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            MetadataToken = Reader.GetConstant(metadataHandle);
            _parent = GetLazyCodeElementWithHandle(MetadataToken.Parent);
            TypeCode = MetadataToken.TypeCode;
            _value = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(MetadataToken.Value)));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.Constant.Parent" />
        /// <summary>The parent handle (<see cref="Parameter" />, <see cref="FieldDefinition" />, or <see cref="PropertyDefinition" />).</summary>
        public CodeElement Parent => _parent.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.Constant.TypeCode" />
        public ConstantTypeCode TypeCode { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.Constant.Value" />
        public Blob Value => _value.Value;

        public Handle DowncastMetadataHandle => MetadataHandle;

        public ConstantHandle MetadataHandle { get; }

        public System.Reflection.Metadata.Constant MetadataToken { get; }
    }
}
