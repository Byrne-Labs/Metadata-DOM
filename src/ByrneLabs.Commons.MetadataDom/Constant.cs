using System;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
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

        public CodeElement Parent => _parent.Value;

        public ConstantTypeCode TypeCode { get; }

        public Blob Value => _value.Value;
    }
}
