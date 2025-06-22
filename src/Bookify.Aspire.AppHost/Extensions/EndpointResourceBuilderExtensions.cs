namespace Bookify.Aspire.AppHost.Extensions;

internal static class EndpointResourceBuilderExtensions
{
    internal static IResourceBuilder<T> WithSwaggerUILink<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return AddUrl(builder, "Swagger", "swagger");
    }

    internal static IResourceBuilder<T> WithScalarLink<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return AddUrl(builder, "Scalar", "scalar/v1");
    }

    internal static IResourceBuilder<T> WithReDocLink<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return AddUrl(builder, "ReDoc", "api-docs");
    }

    private static IResourceBuilder<T> AddUrl<T>(
        IResourceBuilder<T> builder,
        string displayText,
        string urlPath = "swagger"
    )
        where T : IResourceWithEndpoints
    {
        return builder.WithUrls(context =>
        {
            var httpsEndpoint = context.GetHttpEndpoint();
            if (httpsEndpoint?.Url == null)
            {
                throw new InvalidOperationException("No HTTPS endpoint found for the resource.");
            }

            context.Urls.Add(
                new ResourceUrlAnnotation
                {
                    Url = $"{httpsEndpoint.Url}/{urlPath}",
                    DisplayText = displayText,
                }
            );
        });
    }

    private static EndpointReference? GetHttpEndpoint(this ResourceUrlsCallbackContext context)
    {
        if (context.GetEndpoint("https") is { Exists: true } httpsEndpoint)
        {
            return httpsEndpoint;
        }

        if (context.GetEndpoint("http") is { Exists: true } endpoint)
        {
            return endpoint;
        }

        return null;
    }
}
