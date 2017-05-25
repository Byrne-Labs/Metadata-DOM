using System;
using System.Collections.Generic;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom
{
    internal static class KnownHashAlgorithmGuids
    {
        public static readonly Guid Sha1 = Guid.Parse("ff1816ec-aa5e-4d10-87f7-6f4963833460");
        public static readonly Guid Sha256 = Guid.Parse("8829d00f-11b8-4213-878b-770e8597ac16");

        public static readonly IDictionary<Guid, AssemblyHashAlgorithm> MapFromGuid = new Dictionary<Guid, AssemblyHashAlgorithm>
        {
            { Guid.Parse("ff1816ec-aa5e-4d10-87f7-6f4963833460"), AssemblyHashAlgorithm.Sha1 },
            { Guid.Parse("8829d00f-11b8-4213-878b-770e8597ac16"), AssemblyHashAlgorithm.Sha256 },
            { Guid.Parse("406ea660-64cf-4c82-b6f0-42d48172a799"), AssemblyHashAlgorithm.MD5 }
        };

        public static readonly IDictionary<AssemblyHashAlgorithm, Guid> MapToGuid = new Dictionary<AssemblyHashAlgorithm, Guid>
        {
            { AssemblyHashAlgorithm.Sha1, Guid.Parse("ff1816ec-aa5e-4d10-87f7-6f4963833460") },
            { AssemblyHashAlgorithm.Sha256, Guid.Parse("8829d00f-11b8-4213-878b-770e8597ac16") },
            { AssemblyHashAlgorithm.MD5, Guid.Parse("406ea660-64cf-4c82-b6f0-42d48172a799") }
        };
    }
}
