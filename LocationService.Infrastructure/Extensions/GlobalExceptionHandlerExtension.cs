using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace LocationService.Infrastructure.Extensions;

[ExcludeFromCodeCoverage]
public static class GlobalExceptionHandlerExtension
{
    public static IApplicationBuilder UseWebApiExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        var loggerFactory = app.ApplicationServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;

        return app.UseExceptionHandler(HandleApiException(loggerFactory, env));
    }

    private static Action<IApplicationBuilder> HandleApiException(ILoggerFactory loggerFactory, IWebHostEnvironment env)
    {
        return appBuilder =>
        {
            appBuilder.Run(async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (exceptionHandlerFeature != null)
                {
                    var logger = loggerFactory.CreateLogger("GlobalExceptionHandlerExtension");
                    logger.LogError(exceptionHandlerFeature.Error,"Error {Message}", exceptionHandlerFeature.Error.Message);


                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 500;

                    string result;
                    if (!env.IsDevelopment() && exceptionHandlerFeature.Error is not ValidationException)
                    {
                        result = new
                        {
                            Error = $"An unexpected error happened. - Detail: {exceptionHandlerFeature.Error.Message}"
                        }.Serialize();
                    }
                    else
                    {
                        if (exceptionHandlerFeature.Error is ValidationException)
                        {
                            result = new
                            {
                                Error = $"Validation Error - Detail: {exceptionHandlerFeature.Error.Message}"
                            }.Serialize();
                        }
                        else
                        {
                            result = new
                            {
                                Error = exceptionHandlerFeature.Error.Message,
                                Timestamp = DateTime.Now.ToString("yyyy-MM-dd H-m-ss"),
                                Type = exceptionHandlerFeature.Error.GetType().Name
                            }.Serialize();
                        }
                    }

                    await context.Response.WriteAsync(result);
                }
            });
        };
    }
}
