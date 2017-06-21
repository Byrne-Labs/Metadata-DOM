using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using JetBrains.Annotations;
using TypeToExpose = System.Type;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    [PublicAPI]
    [DebuggerDisplay("\\{{GetType().Name,nq}\\}: {Value}")]
    public class Constant : ConstantInfo, IManagedCodeElement
    {
        private readonly Lazy<TypeToExpose> _constantType;
        private readonly Lazy<IManagedCodeElement> _parent;
        private readonly Lazy<object> _value;

        internal Constant(ConstantHandle metadataHandle, MetadataState metadataState)
        {
            MetadataState = metadataState;
            Key = new CodeElementKey<Constant>(metadataHandle);
            MetadataHandle = metadataHandle;
            RawMetadata = MetadataState.AssemblyReader.GetConstant(metadataHandle);
            // ReSharper disable once SwitchStatementMissingSomeCases
            _parent = new Lazy<IManagedCodeElement>(() =>
            {
                IManagedCodeElement parent;
                switch (RawMetadata.Parent.Kind)
                {
                    case HandleKind.Parameter:
                        parent = MetadataState.GetCodeElement<Parameter>(RawMetadata.Parent);
                        break;
                    case HandleKind.FieldDefinition:
                        parent = MetadataState.GetCodeElement<FieldDefinition>(RawMetadata.Parent);
                        break;
                    case HandleKind.PropertyDefinition:
                        parent = MetadataState.GetCodeElement<PropertyDefinition>(RawMetadata.Parent);
                        break;
                    default:
                        throw new ArgumentException($"Unexpected parent type {RawMetadata.Parent.Kind}");
                }

                return parent;
            });
            TypeCode = RawMetadata.TypeCode;
            _constantType = new Lazy<TypeToExpose>(() =>
            {
                Type constantType;
                if (TypeCode == ConstantTypeCode.NullReference)
                {
                    constantType = null;
                }
                else
                {
                    constantType = MetadataState.GetCodeElement<PrimitiveType>((PrimitiveTypeCode) TypeCode);
                }

                return constantType;
            });

            _value = new Lazy<object>(() =>
            {
                object value;
                var blobBytes = MetadataState.AssemblyReader.GetBlobBytes(RawMetadata.Value);
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
                        var byteValue = blobBytes[0];
                        if (byteValue > sbyte.MaxValue)
                        {
                            value = (sbyte) (byteValue - byte.MaxValue - 1);
                        }
                        else
                        {
                            value = (sbyte) byteValue;
                        }
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
                        value = MetadataState.AssemblyReader.UTF8Decoder.Encoding.GetString(blobBytes);
                        break;
                    case ConstantTypeCode.NullReference:
                        value = null;
                        break;
                    case ConstantTypeCode.Invalid:
                        throw new InvalidOperationException("Constant type code is ConstantTypeCode.Invalid");
                    default:
                        throw new InvalidOperationException($"Invalid type code {TypeCode}");
                }

                return value;
            });

            MetadataState.GetLazyCodeElement<TypeBase>((PrimitiveTypeCode) RawMetadata.TypeCode);
        }

        public override TypeToExpose ConstantType => _constantType.Value;

        public ConstantHandle MetadataHandle { get; }

        public System.Reflection.Metadata.Constant RawMetadata { get; }

        public ConstantTypeCode TypeCode { get; }

        public override object Value => _value.Value;

        protected override object Parent => _parent.Value;

        internal CodeElementKey Key { get; }

        internal MetadataState MetadataState { get; }

        CodeElementKey IManagedCodeElement.Key => Key;

        MetadataState IManagedCodeElement.MetadataState => MetadataState;
    }
}
