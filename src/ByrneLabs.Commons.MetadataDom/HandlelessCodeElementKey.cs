using System;

namespace ByrneLabs.Commons.MetadataDom
{
    internal class HandlelessCodeElementKey
    {
        protected HandlelessCodeElementKey(object keyValue, Type codeElementType)
        {
            KeyValue = keyValue;
            CodeElementType = codeElementType;
        }

        public Type CodeElementType { get; }

        public object KeyValue { get; }

        public override bool Equals(object obj)
        {
            var castObj = obj as HandlelessCodeElementKey;
            return ReferenceEquals(this, obj) || castObj != null && castObj.CodeElementType == CodeElementType && castObj.KeyValue.Equals(KeyValue);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 91;
                hash = hash*17 + KeyValue.GetHashCode();
                hash = hash*17 + CodeElementType.GetHashCode();
                return hash;
            }
        }
    }

    internal sealed class HandlelessCodeElementKey<T> : HandlelessCodeElementKey where T : ICodeElementWithoutHandle
    {
        public HandlelessCodeElementKey(object keyValue) : base(keyValue, typeof(T))
        {
        }
    }
}
