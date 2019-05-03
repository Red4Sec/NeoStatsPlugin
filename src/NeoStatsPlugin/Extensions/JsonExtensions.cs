using Newtonsoft.Json;

namespace NeoStatsPlugin.Extensions
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Convert to json
        /// </summary>
        public static string ToJson(this object obj) => JsonConvert.SerializeObject(obj, Formatting.None);
    }
}