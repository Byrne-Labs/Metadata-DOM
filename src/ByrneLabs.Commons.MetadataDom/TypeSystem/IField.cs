namespace ByrneLabs.Commons.MetadataDom.TypeSystem
{
    public interface IField : IMember
    {
        TypeBase FieldType { get; }
    }
}
