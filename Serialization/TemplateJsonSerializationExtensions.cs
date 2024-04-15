using Newtonsoft.Json;

namespace MagmaWorks.Template.Serialization.Extensions
{
    public static class TemplateJsonSerializationExtensions
    {
        public static string ToJson<T>(this T profile) where T : ITemplate
        {
            return JsonConvert.SerializeObject(profile, Formatting.Indented, TemplateJsonSerializer.Settings);
        }

        public static T FromJson<T>(this string json) where T : ITemplate
        {
            return JsonConvert.DeserializeObject<T>(json, TemplateJsonSerializer.Settings);
        }
    }
}
