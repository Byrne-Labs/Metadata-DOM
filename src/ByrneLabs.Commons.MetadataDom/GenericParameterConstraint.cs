using System.Collections.Generic;
using System.Reflection.Metadata;
#if NETSTANDARD2_0 || NET_FRAMEWORK
using TypeToExpose = System.Type;
using CustomAttributeDataToExpose = System.Reflection.CustomAttributeData;

#else
using TypeToExpose = ByrneLabs.Commons.MetadataDom.Type;
using CustomAttributeDataToExpose = ByrneLabs.Commons.MetadataDom.CustomAttributeData;

#endif

namespace ByrneLabs.Commons.MetadataDom
{
    //[PublicAPI]
    public abstract class GenericParameterConstraint
    {
        public abstract IEnumerable<CustomAttributeDataToExpose> CustomAttributes { get; }

        public abstract GenericParameter Parameter { get; }

        public abstract TypeToExpose Type { get; }
    }
}
