namespace ByrneLabs.Commons.MetadataDom
{
    public interface IField : IMember
    {
        TypeBase FieldType { get; }
    }
}