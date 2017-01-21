using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom
{
    // ReSharper disable once UseNameofExpression -- This is a false positive because the method is actually executed
    [DebuggerDisplay("\\{CodeElementKey\\}: {KeysToString()}")]
    internal class CodeElementKey
    {
        public CodeElementKey(Type codeElementType, params object[] keyValues)
        {
            if (keyValues.Length == 0 && codeElementType != typeof(SystemType))
            {
                throw new ArgumentException("At least one key value must be provided", nameof(keyValues));
            }

            var rawKeyValues = keyValues.Where(keyValue => keyValue != null).ToArray();
            for (var index = 0; index < rawKeyValues.Length; index++)
            {
                var handle = MetadataState.DowncastHandle(rawKeyValues[index]);
                if (handle != null)
                {
                    rawKeyValues[index] = handle;
                }
            }

            KeyValues = rawKeyValues.ToImmutableArray();
            Handle = KeyValues.OfType<Handle?>().FirstOrDefault();

            UpcastHandle = Handle.HasValue ? MetadataState.UpcastHandle(Handle.Value) : null;

            if ((codeElementType == null && !typeof(CodeElement).GetTypeInfo().IsAssignableFrom(codeElementType) || codeElementType.GetTypeInfo().IsAbstract) && Handle != null)
            {
                CodeElementType = MetadataState.GetCodeElementTypeForHandle(Handle.Value);
            }
            else if (codeElementType != null && !typeof(CodeElement).GetTypeInfo().IsAssignableFrom(codeElementType))
            {
                throw new ArgumentException($"Type {codeElementType.FullName} does not inherit {typeof(CodeElement).FullName}", nameof(codeElementType));
            }
            else if (codeElementType.GetTypeInfo().IsAbstract)
            {
                throw new ArgumentException($"Type {codeElementType.FullName} is abstract", nameof(codeElementType));
            }
            else if (codeElementType == null)
            {
                throw new ArgumentNullException(nameof(codeElementType));
            }
            else
            {
                CodeElementType = codeElementType;
            }

            var primitiveTypeCode = KeyValues.OfType<PrimitiveTypeCode>().FirstOrDefault();
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

        public ImmutableArray<object> KeyValues { get; }

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
                // ReSharper disable once LoopCanBeConvertedToQuery -- This is much easier to read as a loop. -- Jonathan Byrne 01/21/2017
                foreach (var keyValue in KeyValues)
                {
                    hash = hash * 17 + (keyValue == null ? 0 : keyValue.GetHashCode()) * keyValue.GetType().GetHashCode();
                }

                hash = hash * 17 + CodeElementType.GetHashCode();
                return hash;
            }
        }

        private string KeysToString() => string.Join(", ", KeyValues);
    }

    internal sealed class CodeElementKey<T> : CodeElementKey where T : CodeElement
    {
        public CodeElementKey(params object[] keyValues) : base(typeof(T), keyValues)
        {
        }
    }
}
