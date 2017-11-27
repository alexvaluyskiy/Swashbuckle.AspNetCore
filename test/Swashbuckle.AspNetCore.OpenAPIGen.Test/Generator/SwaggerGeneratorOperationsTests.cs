using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.OpenAPIGen.Model;
using Swashbuckle.AspNetCore.OpenAPIGen.Model.Info;
using Swashbuckle.AspNetCore.OpenAPIGen.Model.Security;
using Xunit;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.OpenAPIGen.Test
{
    public class SwaggerGeneratorOperationsTests
    {
        [Fact]
        public void GetSwagger_TagsActions_AsSpecifiedBySettings()
        {
            var subject = Subject(
                setupApis: apis =>
                {
                    apis.Add("GET", "collection1", nameof(FakeActions.ReturnsEnumerable));
                    apis.Add("GET", "collection2", nameof(FakeActions.ReturnsInt));
                },
                configure: c => c.TagSelector = (apiDesc) => apiDesc.RelativePath);

            var swagger = subject.GetSwagger("v1");

            Assert.Equal(new[] { "collection1" }, swagger.Paths["/collection1"].Get.Tags);
            Assert.Equal(new[] { "collection2" }, swagger.Paths["/collection2"].Get.Tags);
        }

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