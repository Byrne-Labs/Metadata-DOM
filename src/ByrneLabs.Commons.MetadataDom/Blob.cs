using System;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public class Blob
    {
        internal Blob(byte[] bytes)
        {
            Bytes = bytes;
            StringValue = BitConverter.ToString(Bytes);
        }

        public byte[] Bytes { get; }

        public string StringValue { get; }
    }
}
