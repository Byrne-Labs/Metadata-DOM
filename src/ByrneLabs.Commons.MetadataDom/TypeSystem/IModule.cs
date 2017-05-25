namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public interface IModule
    {
        IAssembly Assembly { get; }

        string ScopedName { get; }
    }
}
