using System;
using Swashbuckle.AspNetCore.Swagger;

namespace Microsoft.AspNetCore.Builder
{
    public static class SwaggerBuilderExtensions
    {
        public static IApplicationBuilder UseOpenApi(
            this IApplicationBuilder app,
            Action<SwaggerOptions> setupAction = null)
        {
            var options = new SwaggerOptions();
            setupAction?.Invoke(options);

            return app.UseMiddleware<SwaggerMiddleware>(options);
        }
    }
}