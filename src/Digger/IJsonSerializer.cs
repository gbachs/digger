namespace Digger
{
    public interface IJsonSerializer
    {
        string Serialize<T>(T item);
        T Deserialize<T>(string json);
    }
}