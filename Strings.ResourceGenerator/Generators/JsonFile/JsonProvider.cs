using Newtonsoft.Json;
using Strings.ResourceGenerator.Models;

namespace Strings.ResourceGenerator.Generators.JsonFile;

internal static class JsonProvider
{
    //private static Lazy<JsonSerializerSettings> settings = new Lazy<JsonSerializerSettings>(() => new JsonSerializerSettings() { });
    public static LocalizerGenerator Provide(string path, string clazz, string jsonFile)
    {
        var model = JsonConvert.DeserializeObject<StringsModel>(jsonFile);
        return model.ToGenerator(path, clazz, (d, l) => new JsonGenerator(d, l));
    }
}