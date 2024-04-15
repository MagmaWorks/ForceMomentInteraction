using Newtonsoft.Json;

namespace MagmaWorks.ForceMomentInteraction.Serialization.Extensions
{
    public static class ForceMomentInteractionJsonSerializationExtensions
    {
        public static string ToJson<T>(this T profile) where T : IForceMomentInteraction
        {
            return JsonConvert.SerializeObject(profile, Formatting.Indented, ForceMomentInteractionJsonSerializer.Settings);
        }

        public static T FromJson<T>(this string json) where T : IForceMomentInteraction
        {
            return JsonConvert.DeserializeObject<T>(json, ForceMomentInteractionJsonSerializer.Settings);
        }
    }
}
