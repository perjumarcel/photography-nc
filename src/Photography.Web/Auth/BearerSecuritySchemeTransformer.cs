using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Photography.Web.Auth;

/// <summary>
/// Adds the <c>Bearer</c> security scheme to the OpenAPI document so any consumer
/// (Scalar UI, curl, generated SDKs) knows how to authenticate. Only attached when
/// the JWT bearer scheme is registered with the authentication system.
/// </summary>
public sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider schemes)
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authSchemes = await schemes.GetAllSchemesAsync();
        if (!authSchemes.Any(s => s.Name == "Bearer")) return;

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            In = ParameterLocation.Header,
            BearerFormat = "JWT",
        };
    }
}
