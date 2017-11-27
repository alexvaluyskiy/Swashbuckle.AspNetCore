using Swashbuckle.AspNetCore.SwaggerGen;

namespace Swashbuckle.AspNetCore.OpenAPIGen.Test
{
    public class FakeControllers
    {
        public class NotAnnotated
        {}

        //[SwaggerOperationFilter(typeof(VendorExtensionsOperationFilter))]
        //public class AnnotatedWithSwaggerOperationFilter
        //{ }

        public class TestController
        {}
    }
}