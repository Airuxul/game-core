namespace Air.GameCore.Serialization
{
    public interface IJsonSerializer
    {
        string Serialize(object value);
        System.Collections.Generic.Dictionary<string, object> ParseObject(string json);
        object Deserialize(string json);
        T Deserialize<T>(string json);
    }
}
