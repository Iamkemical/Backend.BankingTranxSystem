using Backend.BankingTranxSystem.API.Extensions;
using Backend.BankingTranxSystem.API.Installers;
using Backend.BankingTranxSystem.SharedServices.ErrorHandling;
using Backend.BankingTranxSystem.SharedServices.Extensions;
using Backend.BankingTranxSystem.SharedServices.Middlewares;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseCustomSerilLog("BankingTranxSystem.ApiClient");
builder.Services.InstallServicesInAssembly(builder.Configuration);

var app = builder.Build();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

app.MapEndpoints();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking Transaction Middleware");

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

// Configure the HTTP request pipeline.
if (app.Environment.IsProduction() == false)
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.ConfigureCustomExceptionMiddleware();

app.UseRouting();

app.UseCors();

//Disabled Response header cache
//app.UseHttpCacheHeaders();

app.UseAuthorization();

app.Run();
