using CourseManagementService.Common;
using CourseManagementService.Database.InitDb;
using CourseManagementService.Extensions;
using CourseManagementService.Middlewares;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

var builder = WebApplication.CreateBuilder(args);
builder.UseCustomLog(Constants.SERVICE_NAME);
builder.AddAutoFact();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();

builder.Services.AddHttpContextAccessor(); // Add IHttpContextAccessor for getting HttpContext in services
builder.Services.AddDataContext(builder.Configuration);
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddCustomHostedServices();

// Setting to use IUrlHelper in services
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped(x =>
{
    var actionContext = x.GetRequiredService<IActionContextAccessor>()
        .ActionContext;
    var factory = x.GetRequiredService<IUrlHelperFactory>();
    return factory.GetUrlHelper(actionContext);
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient();
builder.Services.AddCustomCorsConfig();

builder.Services.AddControllers();
builder.Services.AddAuthorization();

if (builder.Environment.IsProduction())
{
    builder.WebHost.UseUrls("http://0.0.0.0:80");
}

var app = builder.Build();

var dbInit = app.Services.GetRequiredService<IDbInitializer>();
await dbInit.Initialize();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CustomUnauthorizedMiddleware>();
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowSpecificOrigin");
app.MapControllers();

await app.RunAsync();
