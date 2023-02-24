using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace LocationService.API.Extensions;

[ExcludeFromCodeCoverage]
public static class SerilogExtension
{
    public static void UseSerilogLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "Request - {RequestPath}";
                
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                var request = httpContext.Request;

                diagnosticContext.Set("Host", request.Host);
                diagnosticContext.Set("Protocol", request.Protocol);
                diagnosticContext.Set("Scheme", request.Scheme);

                if (request.QueryString.HasValue)
                {
                    diagnosticContext.Set("QueryString", request.QueryString.Value);
                }

                diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

                var endpoint = httpContext.GetEndpoint();
                if (endpoint != null)
                {
                    diagnosticContext.Set("EndpointName", endpoint.DisplayName);
                }
            };
        });
    }
}
