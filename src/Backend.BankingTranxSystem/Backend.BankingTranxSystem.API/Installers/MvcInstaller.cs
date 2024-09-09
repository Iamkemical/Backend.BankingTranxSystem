using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json.Serialization;

namespace Backend.BankingTranxSystem.API.Installers;

public class MvcInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        //services.AddMediatR(typeof(UserManagementDataAccess.AppSettings).Assembly);
        services.AddControllers(setupAction =>
        {
            //setup general indication of response types across controllers
            setupAction.Filters.Add(
                 new ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest));
            setupAction.Filters.Add(
                new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
            setupAction.Filters.Add(
                new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
            setupAction.Filters.Add(
                new ProducesDefaultResponseTypeAttribute());
            setupAction.Filters.Add(
                new ProducesResponseTypeAttribute(StatusCodes.Status401Unauthorized));
            setupAction.Filters.Add(new ProducesAttribute("application/json"));

            //this indicates that if the requested accept header is not supported by the api it should send a 406 (Not acceptable)
            setupAction.ReturnHttpNotAcceptable = true;
            //setupAction.CacheProfiles.Add("240SecondsCacheProfile", new CacheProfile
            //{
            //    Duration = 240
            //});

        })
        //.AddFluentValidation(setupAction => setupAction.RegisterValidatorsFromAssemblyContaining<FluentValidationEntryPoint>())
        .AddNewtonsoftJson(s =>
        {
            s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        })
        .AddXmlDataContractSerializerFormatters()
        .ConfigureApiBehaviorOptions(setupAction =>
        {
            setupAction.InvalidModelStateResponseFactory = context =>
            {
                // create a problem details object
                var problemDetailsFactory = context.HttpContext.RequestServices
                    .GetRequiredService<ProblemDetailsFactory>();
                var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                        context.HttpContext,
                        context.ModelState);

                // add additional info not added by default
                problemDetails.Detail = "See the errors field for details.";
                problemDetails.Instance = context.HttpContext.Request.Path;

                // find out which status code to use
                var actionExecutingContext =
                      context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                // if there are modelstate errors & all keys were correctly
                // found/parsed we're dealing with validation errors
                if ((context.ModelState.ErrorCount > 0) &&
                    (actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
                {
                    problemDetails.Type = "https://transferano.com/modelvalidationproblem";
                    problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                    problemDetails.Title = "One or more validation errors occurred.";

                    return new UnprocessableEntityObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                }

                // if one of the keys wasn't correctly found / couldn't be parsed
                // we're dealing with null/unparsable input
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "One or more errors on input occurred.";
                return new BadRequestObjectResult(problemDetails)
                {
                    ContentTypes = { "application/problem+json" }
                };
            };
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                 builder =>
                 {
                     //only the api gateway is needed on the list
                     builder.AllowAnyOrigin()
                             .AllowAnyHeader()
                             .WithMethods("GET", "POST", "DELETE", "PATCH", "HEAD", "PUT");
                 });
        });
    }
}