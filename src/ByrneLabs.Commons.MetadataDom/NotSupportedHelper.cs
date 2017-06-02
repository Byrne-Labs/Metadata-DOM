using System;

namespace ByrneLabs.Commons.MetadataDom
{
    internal static class NotSupportedHelper
    {
        public static NotSupportedException FutureVersion() => new NotSupportedException("This method will be supported in a future version");

        public static NotSupportedException NotValidForMetadata() => new NotSupportedException("This method is not valid on metadata");

        public static NotSupportedException NotValidForMetadataType(Type type) => new NotSupportedException($"This method is not valid on metadata of type {type.Name}");
    }
}
