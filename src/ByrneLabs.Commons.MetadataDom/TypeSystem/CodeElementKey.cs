using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    // ReSharper disable once UseNameofExpression -- This is a false positive because the method is actually executed
    [DebuggerDisplay("\\{CodeElementKey\\}: {KeysToString()}")]
    internal class CodeElementKey
    {
        public CodeElementKey(System.Type codeElementType, params object[] keyValues)
        {
            if (keyValues.Length == 0 && codeElementType != typeof(SystemType) && codeElementType != typeof(SystemArray))
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
            var declaredHandle = KeyValues.OfType<Handle?>().FirstOrDefault();
            if (declaredHandle == null && KeyValues.OfType<TypeBase>().Any())
            {
                declaredHandle = KeyValues.OfType<TypeBase>().First().DowncastMetadataHandle;
            }
            Handle = declaredHandle;

            UpcastHandle = Handle.HasValue ? MetadataState.UpcastHandle(Handle.Value) : null;

            if ((codeElementType == null && !typeof(IManagedCodeElement).GetTypeInfo().IsAssignableFrom(codeElementType) || codeElementType.GetTypeInfo().IsAbstract) && Handle != null)
            {
                CodeElementType = MetadataState.GetCodeElementTypeForHandle(Handle.Value);
            }
            else if (codeElementType != null && !typeof(IManagedCodeElement).GetTypeInfo().IsAssignableFrom(codeElementType))
            {
                throw new ArgumentException($"Type {codeElementType.FullName} does not inherit {typeof(IManagedCodeElement).FullName}", nameof(codeElementType));
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

        public System.Type CodeElementType { get; }

        public Handle? Handle { get; }

        public ImmutableArray<object> KeyValues { get; }

        public PrimitiveTypeCode? PrimitiveTypeCode { get; }

        public object UpcastHandle { get; }

        public override bool Equals(object obj)
        {
            var castObj = obj as CodeElementKey;
            var equals = ReferenceEquals(this, obj) || castObj != null && castObj.CodeElementType == CodeElementType && castObj.KeyValues.Length == KeyValues.Length;
            if (equals)
            {
                /*
                 * :( ArrayShape does not implement the equals method to compare the actual values so we have to manually do it. -- Jonathan Byrne 02/01/2017
                 */
                if (KeyValues.OfType<ArrayShape>().Any())
                {
                    for (var keyValueIndex = 0; keyValueIndex < KeyValues.Length && equals; keyValueIndex++)
                    {
                        var thisKeyValue = KeyValues[keyValueIndex];
                        var otherKeyValue = castObj.KeyValues[keyValueIndex];
                        if (thisKeyValue is ArrayShape && otherKeyValue is ArrayShape)
                        {
                            var thisArrayShape = (ArrayShape) thisKeyValue;
                            var otherArrayShape = (ArrayShape) otherKeyValue;
                            equals = thisArrayShape.Rank == otherArrayShape.Rank && Enumerable.SequenceEqual(thisArrayShape.LowerBounds, otherArrayShape.LowerBounds) && Enumerable.SequenceEqual(thisArrayShape.Sizes, otherArrayShape.Sizes);
                        }
                        else
                        {
                            equals = Equals(thisKeyValue, otherKeyValue);
                        }

                    }
                }
                else
                {
                    equals = Enumerable.SequenceEqual(KeyValues, castObj.KeyValues);
                }
            }

            return equals;
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

    internal sealed class CodeElementKey<T> : CodeElementKey where T : IManagedCodeElement
    {
        public CodeElementKey(params object[] keyValues) : base(typeof(T), keyValues)
        {
        }
    }
}
