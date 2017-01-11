using System;

namespace ByrneLabs.Commons.MetadataDom
{
    public class BadMetadataException : Exception
    {
        public BadMetadataException()
        {
        }

        public BadMetadataException(string message) : base(message)
        {
        }

        public BadMetadataException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
