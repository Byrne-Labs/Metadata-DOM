using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    internal class CodeElementKey
    {
        public CodeElementKey(Type codeElementType, params object[] keyValues)
        {
            if (keyValues.Length == 0)
            {
                throw new ArgumentException("At least one key value must be provided", nameof(keyValues));
            }
            if (keyValues.Any(keyValue => keyValue == null))
            {
                throw new ArgumentException("Key values cannot be null", nameof(keyValues));
            }
            if (codeElementType != null && !typeof(CodeElement).GetTypeInfo().IsAssignableFrom(codeElementType))
            {
                throw new ArgumentException($"Type {codeElementType.FullName} does not inherit {typeof(CodeElement).FullName}", nameof(codeElementType));
            }

            var rawKeyValues = (object[])keyValues.Clone();
            for (var index = 0; index < rawKeyValues.Length; index++)
            {
                var handle = MetadataState.DowncastHandle(rawKeyValues[index]);
                if (handle != null)
                {
                    rawKeyValues[index] = handle;
                }
            }
            KeyValues = rawKeyValues;
            // ReSharper disable once ReplaceWithOfType.3 -- NOTE: using OfType<Handle> will convert any integer value to a handle
            var handles = KeyValues.Where(keyValue => keyValue is Handle).Select(keyValue => (Handle)keyValue);
            Handle = KeyValues.OfType<Handle?>().FirstOrDefault();

            UpcastHandle = Handle.HasValue ? MetadataState.UpcastHandle(Handle.Value) : null;

            if (codeElementType.GetTypeInfo().IsAbstract)
            {
                if (Handle == null)
                {
                    throw new ArgumentException($"Type {CodeElementType.FullName} is abstract", nameof(codeElementType));
                }
                CodeElementType = MetadataState.GetCodeElementTypeForHandle(Handle.Value);
            }
            else if (codeElementType == null)
            {
                if (Handle == null)
                {
                    throw new ArgumentNullException(nameof(codeElementType));
                }
                CodeElementType = MetadataState.GetCodeElementTypeForHandle(Handle.Value);
            }
            else
            {
                CodeElementType = codeElementType;
            }

            var primitiveTypeCode = keyValues.OfType<PrimitiveTypeCode>().FirstOrDefault();
            PrimitiveTypeCode = primitiveTypeCode == 0 ? (PrimitiveTypeCode?)null : primitiveTypeCode;
        }

        public CodeElementKey(Handle handle) : this(MetadataState.GetCodeElementTypeForHandle(handle), handle)
        {
        }

        public CodeElementKey(object handle) : this(MetadataState.DowncastHandle(handle) ?? throw new ArgumentException(nameof(handle)))
        {
        }

        public Type CodeElementType { get; }

        public Handle? Handle { get; }

        public IEnumerable<object> KeyValues { get; }

        public PrimitiveTypeCode? PrimitiveTypeCode { get; }

        public object UpcastHandle { get; }

        public override bool Equals(object obj)
        {
            var castObj = obj as CodeElementKey;
            return ReferenceEquals(this, obj) || castObj != null && castObj.CodeElementType == CodeElementType && castObj.KeyValues.SequenceEqual(KeyValues);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 91;
                foreach (var keyValue in KeyValues)
                {
                    hash = hash * 17 + keyValue.GetHashCode();
                }
                hash = hash * 17 + CodeElementType.GetHashCode();
                return hash;
            }
        }
    }

    internal sealed class CodeElementKey<T> : CodeElementKey where T : CodeElement
    {
        public CodeElementKey(params object[] keyValues) : base(typeof(T), keyValues)
        {
        }
    }
}
