using System;
using JetBrains.Annotations;

namespace ByrneLabs.Commons.MetadataDom
{
    [PublicAPI]
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
