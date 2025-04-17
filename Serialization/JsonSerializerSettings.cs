using Newtonsoft.Json;

namespace MagmaWorks.ForceMomentInteraction.Serialization
{
    public static class ForceMomentInteractionJsonSerializer
    {
        public static JsonSerializerSettings Settings
        {
            get
            {
                var settings = new JsonSerializerSettings
                {
                    Converters = {
                        new UnitsNet.Serialization.JsonNet.UnitsNetIQuantityJsonConverter(),
                    },
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                };
                return settings;
            }
        }
    }
}
