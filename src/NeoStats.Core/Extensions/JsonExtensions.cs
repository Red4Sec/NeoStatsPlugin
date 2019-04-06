using Newtonsoft.Json;

namespace NeoStats.Core.Extensions
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Convert to json
        /// </summary>
        public static string ToJson(this object obj) => JsonConvert.SerializeObject(obj, Formatting.None);
    }
}