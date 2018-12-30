using Microsoft.AspNetCore.Builder;

namespace Graphity.Middleware
{
    /// <summary>
    /// Extension methods for configuring the Graphity middleware.
    /// </summary>
    public static class GraphityMiddlewareExtensions
    {
        /// <summary>
        /// Add the Graphity middleware to the ASP pipeline
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="path">Optional parameter to specify the URL that is used for the graph endpoint</param>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static IApplicationBuilder UseGraphity(this IApplicationBuilder builder,
            string path = "/api/graph")
        {
            builder.Map(path, applicationBuilder =>
            {
                applicationBuilder.UseMiddleware<GraphityMiddleware>();
            });

            return builder;
        }
    }
}