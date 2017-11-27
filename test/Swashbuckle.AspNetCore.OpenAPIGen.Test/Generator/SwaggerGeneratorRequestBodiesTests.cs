using System;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.OpenAPIGen.Model.Info;
using Xunit;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.OpenAPIGen.Test
{
    public class SwaggerGeneratorRequestBodiesTests
    {
        //[Fact]
        //public void GetSwagger_GeneratesBodyParams_ForBodyBoundParams()
        //{
        //    var subject = Subject(setupApis: apis => apis
        //        .Add("POST", "collection", nameof(FakeActions.AcceptsComplexTypeFromBody)));

        //    var swagger = subject.GetSwagger("v1");

        //    var param = swagger.Paths["/collection"].Post.Parameters.First();
        //    Assert.IsAssignableFrom<BodyParameter>(param);
        //    var bodyParam = param as BodyParameter;
        //    Assert.Equal("param", bodyParam.Name);
        //    Assert.Equal("body", bodyParam.In);
        //    Assert.NotNull(bodyParam.Schema);
        //    Assert.Equal("#/definitions/ComplexType", bodyParam.Schema.Ref);
        //    Assert.Contains("ComplexType", swagger.Definitions.Keys);
        //}

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