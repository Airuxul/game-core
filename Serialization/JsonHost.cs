using System;
using System.Collections.Generic;

namespace Air.GameCore.Serialization
{
    public static class JsonHost
    {
        public static IJsonSerializer Instance { get; set; }

        public static string Serialize(object value) => Require().Serialize(value);

        public static Dictionary<string, object> ParseObject(string json) => Require().ParseObject(json);

        public static object Deserialize(string json) => Require().Deserialize(json);

        static IJsonSerializer Require()
        {
            if (Instance == null)
            {
                throw new InvalidOperationException(
                    "JsonHost.Instance is not set. Register IJsonSerializer from com.air.unity-game-core at startup.");
            }

            return Instance;
        }
    }
}
