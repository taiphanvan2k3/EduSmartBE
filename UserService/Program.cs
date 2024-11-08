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
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "user-service/swagger/{documentName}/swagger.json";
    });


    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/user-service/swagger/v1/swagger.json", "User API V1");
        options.RoutePrefix = "user-service/swagger"; // Prefix cho Swagger UI
        options.InjectStylesheet("/user-service/swagger/custom-swagger.css"); // Đường dẫn cho file CSS
        options.InjectJavascript("/user-service/swagger/custom-swagger.js"); // Đường dẫn cho file JavaScript
    });
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowSpecificOrigin");

app.UseStaticFiles();

// Đặt trước middleware Authentication và Authorization để mới có thể handle response khi không được phép truy cập 
app.UseMiddleware<CustomUnauthorizedMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<GrpcUserService>();
app.MapControllers();

await app.RunAsync();
