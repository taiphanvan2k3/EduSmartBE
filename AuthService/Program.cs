using AuthService.Commons;
using AuthService.Databases.InitDb;
using AuthService.Extensions;
using AuthService.Middlewares;
using AuthService.Services.MailSender;
using AuthService.Settings;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

var builder = WebApplication.CreateBuilder(args);
builder.UseCustomLog(Constants.SERVICE_NAME);
builder.AddAutoFact();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();

builder.Services.Configure<ServerSetting>(builder.Configuration.GetSection("ServerSetting"));
builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSetting"));
builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JWTSetting"));

// Add services
// builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
// builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
// builder.Services.AddGrpc();

builder.Services.AddDataContext(builder.Configuration);
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddMyIdentityConfig();
builder.Services.AddCustomAuthentication(builder.Configuration);

builder.Services.AddCustomHostedServices();

// Setting to use Razor view rendering
builder.Services.AddRazorPages();
builder.Services.AddScoped<RazorViewService>(); // Add scopes because RazorViewToStringRenderer has dependencies with scoped services

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

builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();

var dbInit = app.Services.GetRequiredService<IDbInitializer>();
if (app.Environment.IsProduction())
{
    await dbInit.Migrate();
}

await dbInit.Initialize();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CustomUnauthorizedMiddleware>();
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// app.MapGrpcService<GrpcPlatformService>();

app.MapGet("/protos/platforms.proto", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

await app.RunAsync();
