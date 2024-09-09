using Backend.BankingTranxSystem.SharedServices.Helper;

namespace Backend.BankingTranxSystem.API.Installers;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.InstallServicesInAssembly(Configuration);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (Utility.GetEnvironmentName() != Utility.PRODUCTION)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");

                //configure UI - Changes some default layout
                c.DefaultModelExpandDepth(2);
                //defaults the response section to Model instead of Example values
                c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                //default is List, it collapses all sections
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                //enables browsing to an operation via url
                c.EnableDeepLinking();
                //displays the operation id
                c.DisplayOperationId();
            });

            app.UseStaticFiles();
        }

        app.UseHttpsRedirection();


        app.UseRouting();

        app.UseCors();

        app.UseAuthorization();
    }
}
