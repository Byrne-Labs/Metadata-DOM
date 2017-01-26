namespace ByrneLabs.Commons.MetadataDom.Tests.Checker
{
    public enum CheckPhase
    {
        AssemblyLoad = 1,
        MetadataLoad = 2,
        MetadataCheck = 3,
        ReflectionComparison = 4,
        MoveAssembly = 5
    }
}
