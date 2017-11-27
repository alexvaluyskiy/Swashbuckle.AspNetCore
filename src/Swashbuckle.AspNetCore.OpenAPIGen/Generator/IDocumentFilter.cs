using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.OpenAPIGen.Model;
using Swashbuckle.AspNetCore.Swagger;

namespace Swashbuckle.AspNetCore.SwaggerGen
{
    public interface IDocumentFilter
    {
        void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context);
    }

    public class DocumentFilterContext
    {
        public DocumentFilterContext(
            ApiDescriptionGroupCollection apiDescriptionsGroups,
            ISchemaRegistry schemaRegistry)
        {
            ApiDescriptionsGroups = apiDescriptionsGroups;
            SchemaRegistry = schemaRegistry;
        }

        public ApiDescriptionGroupCollection ApiDescriptionsGroups { get; private set; }

        public ISchemaRegistry SchemaRegistry { get; private set; }
    }
}
