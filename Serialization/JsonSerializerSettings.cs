using Newtonsoft.Json;

namespace MagmaWorks.Template.Serialization
{
    public static class TemplateJsonSerializer
    {
        public static JsonSerializerSettings Settings
        {
            get
            {
                var settings = new JsonSerializerSettings
                {
                    Converters = {
                        new OasysUnits.Serialization.JsonNet.OasysUnitsIQuantityJsonConverter(),
                    },
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                };
                return settings;
            }
        }
    }
}
