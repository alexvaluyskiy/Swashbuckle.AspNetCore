using System.Collections.Generic;
using Swashbuckle.AspNetCore.OpenAPIGen.Model;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.OpenAPIGen.Test
{
    public class RecursiveCallSchemaFilter : ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {
            model.Properties = new Dictionary<string, Schema>();
            model.Properties.Add("ExtraProperty", context.SchemaRegistry.GetOrRegister(typeof(ComplexType)));
        }
    }
}
