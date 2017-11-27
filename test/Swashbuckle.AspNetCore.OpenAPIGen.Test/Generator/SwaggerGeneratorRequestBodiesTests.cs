﻿using System;
using System.Linq;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.OpenAPIGen.Model.Info;
using Xunit;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.OpenAPIGen.Test
{
    public class SwaggerGeneratorRequestBodiesTests
    {
        [Fact]
        public void GetSwagger_GeneratesBodyParams_ForBodyBoundParams()
        {
            var subject = Subject(setupApis: apis => apis
                .Add("POST", "collection", nameof(FakeActions.AcceptsComplexTypeFromBody)));

            var swagger = subject.GetSwagger("v1");

            var operation = swagger.Paths["/collection"].Post;
            Assert.NotNull(operation);
            Assert.NotNull(operation.RequestBody);
            Assert.True(operation.RequestBody.Content.ContainsKey("application/json"));
            var content = operation.RequestBody.Content["application/json"];
            Assert.NotNull(content.Schema);
            Assert.Equal("#/components/schemas/ComplexType", content.Schema.Ref);
            //Assert.Contains("ComplexType", swagger.Components.Definitions.Keys);
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