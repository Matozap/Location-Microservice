using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace LocationService.API.Helpers.Middleware;

public static class SwaggerConfigurationExtension
{
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Location Service",
                Version = "v1",
                Description = "Location Service API",
                TermsOfService = new Uri("https://matozap.com"),
                Contact = new OpenApiContact
                {
                    Name = "Sluk4d"
                }
            });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        });
    }

    public static void UseSwaggerApi(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Location Service");
            c.DisplayRequestDuration();
            c.DefaultModelExpandDepth(1);
            c.DefaultModelsExpandDepth(-1);
        });
    }
}
