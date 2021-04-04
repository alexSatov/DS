using Newtonsoft.Json;

namespace DS.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object obj, Formatting formatting = Formatting.Indented)
        {
            return JsonConvert.SerializeObject(obj, formatting);
        }
    }
}
