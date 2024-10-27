using UserService.Commons;
using UserService.Databases.InitDb;
using UserService.Extensions;
using UserService.Middlewares;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using UserService.EventProcessing;
using UserService.AsyncDataServices;
using UserService.GrpcServices;

var builder = WebApplication.CreateBuilder(args);
builder.UseCustomLog(Constants.SERVICE_NAME);
builder.AddAutoFact();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();
builder.Services.AddCloudinarySettings(builder.Configuration);

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddDataContext(builder.Configuration);
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddCustomAuthentication(builder.Configuration);

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
builder.Services.AddCustomCorsConfig();

builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.AddGrpc();
var app = builder.Build();

var dbInit = app.Services.GetRequiredService<IDbInitializer>();
await dbInit.Initialize();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.InjectStylesheet("/swagger/custom-swagger.css");
        options.InjectJavascript("/swagger/custom-swagger.js");
    });
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CustomUnauthorizedMiddleware>();
app.MapGrpcService<GrpcUserService>();
app.MapControllers();

await app.RunAsync();
