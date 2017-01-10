using System;

namespace ByrneLabs.Commons.MetadataDom
{
    public class BadImageException : Exception
    {
        public BadImageException()
        {
        }

        public BadImageException(string message) : base(message)
        {
        }

        public BadImageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
