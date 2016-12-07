using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.Constant" />
    public class Constant : CodeElementWithHandle
    {
        private readonly Lazy<CodeElement> _parent;
        private readonly Lazy<Blob> _value;

        internal Constant(ConstantHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            var constant = Reader.GetConstant(metadataHandle);
            _parent = new Lazy<CodeElement>(() => GetCodeElement(constant.Parent));
            TypeCode = constant.TypeCode;
            _value = new Lazy<Blob>(() => new Blob(Reader.GetBlobBytes(constant.Value)));
        }

        /// <inheritdoc cref="System.Reflection.Metadata.Constant.Parent" />
        /// <summary>The parent handle (<see cref="T:ByrneLabs.Commons.MetadataDom.Parameter" />, <see cref="T:ByrneLabs.Commons.MetadataDom.FieldDefinition" />, or
        ///     <see cref="T:ByrneLabs.Commons.MetadataDom.PropertyDefinition" />).</summary>
        public CodeElement Parent => _parent.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.Constant.TypeCode" />
        public ConstantTypeCode TypeCode { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.Constant.Value" />
        public Blob Value => _value.Value;
    }
}
