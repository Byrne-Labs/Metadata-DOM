using System;

namespace ByrneLabs.Commons.MetadataDom
{
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
