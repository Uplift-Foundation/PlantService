using PlantService.Data;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DotNetEnv;
using FMN.Vault;

// Load secrets from Vault if enabled, otherwise fall back to .env file
var vaultEnabled = Environment.GetEnvironmentVariable("VAULT_ENABLED") ?? "false";
if (vaultEnabled.Equals("true", StringComparison.OrdinalIgnoreCase))
{
    var vaultClient = new FmnVaultClient("plantservice");
    await vaultClient.LoadSecretsIntoEnvironmentAsync();
}
else
{
    // Fall back to .env file for local dev without Vault
    Env.Load();
}

var builder = WebApplication.CreateBuilder(args);

// Get connection string from configuration
var connectionString = (Environment.GetEnvironmentVariable("ConnectionString__DefaultConnection")
    ?? Environment.GetEnvironmentVariable("ConnectionStringsDefaultConnection"))
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

// Normalize environment variables
var keycloakAuthority = Environment.GetEnvironmentVariable("Keycloak__Authority")
    ?? Environment.GetEnvironmentVariable("KeycloakAuthority");
var keycloakAudience = Environment.GetEnvironmentVariable("Keycloak__Audience")
    ?? Environment.GetEnvironmentVariable("KeycloakAudience");
var keycloakOAuthClientId = Environment.GetEnvironmentVariable("Keycloak__OAuthClientId")
    ?? Environment.GetEnvironmentVariable("KeycloakOAuthClientId");
var keycloakOAuthClientSecret = Environment.GetEnvironmentVariable("Keycloak__OAuthClientSecret")
    ?? Environment.GetEnvironmentVariable("KeycloakOAuthClientSecret");

if (!string.IsNullOrEmpty(keycloakAuthority))
    Environment.SetEnvironmentVariable("KeycloakAuthority", keycloakAuthority);
if (!string.IsNullOrEmpty(keycloakAudience))
    Environment.SetEnvironmentVariable("KeycloakAudience", keycloakAudience);
if (!string.IsNullOrEmpty(keycloakOAuthClientId))
    Environment.SetEnvironmentVariable("KeycloakOAuthClientId", keycloakOAuthClientId);
if (!string.IsNullOrEmpty(keycloakOAuthClientSecret))
    Environment.SetEnvironmentVariable("KeycloakOAuthClientSecret", keycloakOAuthClientSecret);
if (!string.IsNullOrEmpty(connectionString))
    Environment.SetEnvironmentVariable("ConnectionStringsDefaultConnection", connectionString);

builder.Services.AddHttpContextAccessor();

// Add auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = Environment.GetEnvironmentVariable("KeycloakAuthority");
        options.Audience = Environment.GetEnvironmentVariable("KeycloakAudience");
    });

builder.Services.AddControllers();
builder.Services.AddHttpClient();

var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
    options.ApiVersionReader = new QueryStringApiVersionReader();
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services.AddDbContext<PlantContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("ConnectionStringsDefaultConnection")));

apiVersioningBuilder.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddServer(new OpenApiServer { Url = "https://localhost:7090", Description = "Local dotnet run Server (HTTPS)" });
    options.AddServer(new OpenApiServer { Url = "http://localhost:5090", Description = "Local dotnet run Server (HTTP)" });
    options.AddServer(new OpenApiServer { Url = "http://localhost:90", Description = "Local Docker Compose Server" });
    options.AddServer(new OpenApiServer { Url = "https://localhost/plant", Description = "Local Kubernetes Server" });
    options.AddServer(new OpenApiServer { Url = "https://forgetmenotqa.uplifttech.org/plant", Description = "Cloud QA Server" });
    options.AddServer(new OpenApiServer { Url = "https://forgetmenot.uplifttech.org/plant", Description = "Cloud Prod Server" });

#pragma warning disable ASP0000
    var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
#pragma warning restore ASP0000

    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerDoc(description.GroupName, new OpenApiInfo()
        {
            Title = $"Forget Me Not Plant API {description.ApiVersion}",
            Version = description.ApiVersion.ToString(),
        });
    }

    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{Environment.GetEnvironmentVariable("KeycloakAuthority")}/protocol/openid-connect/auth"),
                TokenUrl = new Uri($"{Environment.GetEnvironmentVariable("KeycloakAuthority")}/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "Open ID" }
                }
            }
        }
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { "openid" }
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"dotnet run only: {description.GroupName.ToUpperInvariant()}");
        options.SwaggerEndpoint($"/plant/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
    }
    options.OAuthClientId(Environment.GetEnvironmentVariable("KeycloakOAuthClientId"));
    options.OAuthAppName("Forget Me Not Plant Service Swagger UI");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PlantContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        logger.LogError(ex, "Connection String: " + Environment.GetEnvironmentVariable("ConnectionStringsDefaultConnection"));
    }
}

app.Run();
