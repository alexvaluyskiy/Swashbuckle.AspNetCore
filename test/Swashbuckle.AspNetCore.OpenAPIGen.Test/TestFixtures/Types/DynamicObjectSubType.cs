using System.Dynamic;
using Newtonsoft.Json;

namespace Swashbuckle.AspNetCore.OpenAPIGen.Test
{
    [JsonObject]
    public class DynamicObjectSubType : DynamicObject
    {
        public string Property1 { get; set; }
    }
}