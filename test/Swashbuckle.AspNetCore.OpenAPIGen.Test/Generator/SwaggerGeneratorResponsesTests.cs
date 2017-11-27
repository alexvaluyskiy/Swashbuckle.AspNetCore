using System;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.OpenAPIGen.Model.Info;
using Xunit;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.OpenAPIGen.Test
{
    public class SwaggerGeneratorResponsesTests
    {
        private SwaggerGenerator Subject(
            Action<FakeApiDescriptionGroupCollectionProvider> setupApis = null,
            Action<SwaggerGeneratorSettings> configure = null)
        {
            var apiDescriptionsProvider = new FakeApiDescriptionGroupCollectionProvider();
            setupApis?.Invoke(apiDescriptionsProvider);

            var options = new SwaggerGeneratorSettings();
            options.SwaggerDocs.Add("v1", new Info { Title = "API", Version = "v1" });

            configure?.Invoke(options);

            return new SwaggerGenerator(
                apiDescriptionsProvider,
                new SchemaRegistryFactory(new JsonSerializerSettings(), new SchemaRegistrySettings()),
                options
            );
        }
    }
}