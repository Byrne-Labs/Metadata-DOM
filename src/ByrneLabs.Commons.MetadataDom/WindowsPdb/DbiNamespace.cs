// dnlib: See LICENSE.txt for more info

namespace ByrneLabs.Commons.MetadataDom.WindowsPdb
{
    public sealed class DbiNamespace
    {
        public DbiNamespace(string ns)
        {
            Namespace = ns;
        }

        public string Namespace { get; private set; }


        public string Name
        {
            get
            {
                return Namespace;
            }
        }

    }
}
