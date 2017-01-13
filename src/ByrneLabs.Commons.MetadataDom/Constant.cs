using System;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    /// <inheritdoc cref="System.Reflection.Metadata.Constant" />
    //[PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Parent.FullName,nq} = {Value}")]
    public class Constant : RuntimeCodeElement, ICodeElementWithTypedHandle<ConstantHandle, System.Reflection.Metadata.Constant>
    {
        private readonly Lazy<IMember> _parent;
        private readonly Lazy<object> _value;

        internal Constant(ConstantHandle metadataHandle, MetadataState metadataState) : base(metadataHandle, metadataState)
        {
            MetadataHandle = metadataHandle;
            RawMetadata = Reader.GetConstant(metadataHandle);
            _parent = MetadataState.GetLazyCodeElement<IMember>(RawMetadata.Parent);
            TypeCode = RawMetadata.TypeCode;
            _value = new Lazy<object>(() => new Blob(Reader.GetBlobBytes(RawMetadata.Value)));

            _value = new Lazy<object>(() =>
            {
                object value;
                var blobBytes = Reader.GetBlobBytes(RawMetadata.Value);
                switch (TypeCode)
                {
                    case ConstantTypeCode.Byte:
                        value = blobBytes[0];
                        break;
                    case ConstantTypeCode.Boolean:
                        value = BitConverter.ToBoolean(blobBytes, 0);
                        break;
                    case ConstantTypeCode.Char:
                        value = BitConverter.ToChar(blobBytes, 0);
                        break;
                    case ConstantTypeCode.SByte:
                        value = Convert.ToSByte(blobBytes[0]);
                        break;
                    case ConstantTypeCode.Int16:
                        value = BitConverter.ToInt16(blobBytes, 0);
                        break;
                    case ConstantTypeCode.UInt16:
                        value = BitConverter.ToUInt16(blobBytes, 0);
                        break;
                    case ConstantTypeCode.Int32:
                        value = BitConverter.ToInt32(blobBytes, 0);
                        break;
                    case ConstantTypeCode.UInt32:
                        value = BitConverter.ToUInt32(blobBytes, 0);
                        break;
                    case ConstantTypeCode.Int64:
                        value = BitConverter.ToInt64(blobBytes, 0);
                        break;
                    case ConstantTypeCode.UInt64:
                        value = BitConverter.ToUInt64(blobBytes, 0);
                        break;
                    case ConstantTypeCode.Single:
                        value = BitConverter.ToSingle(blobBytes, 0);
                        break;
                    case ConstantTypeCode.Double:
                        value = BitConverter.ToDouble(blobBytes, 0);
                        break;
                    case ConstantTypeCode.String:
                        value = Reader.UTF8Decoder.Encoding.GetString(blobBytes);
                        break;
                    case ConstantTypeCode.NullReference:
                        value = null;
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid type code {TypeCode}");
                }

                return value;
            });

            MetadataState.GetLazyCodeElement<TypeBase>((PrimitiveTypeCode) RawMetadata.TypeCode);
        }

        /// <inheritdoc cref="System.Reflection.Metadata.Constant.Parent" />
        /// <summary>The parent handle (<see cref="Parameter" />, <see cref="FieldDefinition" />, or <see cref="PropertyDefinition" />).</summary>
        public IMember Parent => _parent.Value;

        /// <inheritdoc cref="System.Reflection.Metadata.Constant.TypeCode" />
        public ConstantTypeCode TypeCode { get; }

        /// <inheritdoc cref="System.Reflection.Metadata.Constant.Value" />
        public object Value => _value.Value;

        public System.Reflection.Metadata.Constant RawMetadata { get; }

        public ConstantHandle MetadataHandle { get; }
    }
}
