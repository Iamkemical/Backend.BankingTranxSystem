using Backend.BankingTranxSystem.SharedServices.Helper;
using Backend.BankingTranxSystem.SharedServices.Helper.Swagger;
using Microsoft.OpenApi.Models;

namespace Backend.BankingTranxSystem.API.Installers;

public class SwaggerInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Banking Transaction Middleware", Version = "v1" });
            c.CustomSchemaIds(type => Guid.NewGuid().ToString());
            c.SchemaFilter<EnumTypesSchemaFilter>();
            c.DocumentFilter<EnumTypesDocumentFilter>();

            if (Utility.GetEnvironmentName() == Utility.DEVELOPMENT)
            {
                c.AddServer(new OpenApiServer
                {
                    Url = "https://localhost:7146"
                });
            }

            //var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
            //c.IncludeXmlComments(xmlCommentsFullPath);
            //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Backend.BankingTranxSystem.API.xml"));
            //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "CamMgt.Shared.xml"));

            c.AddSecurityDefinition("requestId", new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = "X-REQ-ID",
                Scheme = "UserRequestIdAuthAttribute",
                Description = "Enter your request id to authorize"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "requestId"
                        }
                    }, new List<string>()
                }
            });
        });
    }
}