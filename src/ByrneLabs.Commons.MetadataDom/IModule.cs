namespace ByrneLabs.Commons.MetadataDom
{
    public interface IModule
    {
        IAssembly Assembly { get; }

        string ScopedName{ get; }
    }
}
