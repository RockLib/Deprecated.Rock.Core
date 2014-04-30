namespace Rock.Serialization
{
    public interface IJsonSerializer
    {
        string Serialize(object item);
    }
}