using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;

namespace TodoApi.Filters {
    class AuthOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext ctx)
        {
            if (ctx.ApiDescription.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                // If not [AllowAnonymous] and [Authorize] on either the endpoint or the controller...
                if (!ctx.ApiDescription.CustomAttributes().Any((a) => a is AllowAnonymousAttribute)
                    && (ctx.ApiDescription.CustomAttributes().Any((a) => a is AuthorizeAttribute)
                        || descriptor.ControllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>() != null))
                {
                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        }] = Array.Empty<string>()
                    });
                }
            }
        }
    }
}