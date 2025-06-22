using System.Diagnostics;

namespace Bookify.Aspire.AppHost.Extensions;

internal static class ApiResourceBuilderExtensions
{
    internal static IResourceBuilder<T> WithSwaggerUI<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return builder.WithOpenApiDocs("swagger", "Swagger", "swagger");
    }

    internal static IResourceBuilder<T> WithScalar<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return builder.WithOpenApiDocs("scalar", "Scalar", "scalar/v1");
    }

    internal static IResourceBuilder<T> WithReDoc<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        return builder.WithOpenApiDocs("redoc", "ReDoc", "api-docs");
    }

    private static IResourceBuilder<T> WithOpenApiDocs<T>(
        this IResourceBuilder<T> builder,
        string name,
        string displayName,
        string openApiUiPath
    )
        where T : IResourceWithEndpoints
    {
        return builder.WithCommand(
            name,
            displayName,
            executeCommand: _ =>
            {
                return ExecuteOpenApiUrl(builder, openApiUiPath);
            },
            new CommandOptions { IconVariant = IconVariant.Filled, IconName = "Document" }
        );
    }

    private static Task<ExecuteCommandResult> ExecuteOpenApiUrl<T>(
        IResourceBuilder<T> builder,
        string openApiUiPath
    )
        where T : IResourceWithEndpoints
    {
        try
        {
            var endpoint = builder.GetEndpoint("https");

            if (endpoint?.Url == null)
            {
                return Task.FromResult(
                    new ExecuteCommandResult
                    {
                        Success = false,
                        ErrorMessage = "No HTTPS endpoint found for the resource.",
                    }
                );
            }

            var url = $"{endpoint.Url}/{openApiUiPath}";

            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });

            return Task.FromResult(new ExecuteCommandResult { Success = true });
        }
        catch (Exception e)
        {
            return Task.FromResult(
                new ExecuteCommandResult() { Success = false, ErrorMessage = e.ToString() }
            );
        }
    }
}
