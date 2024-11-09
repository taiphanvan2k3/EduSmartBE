using System.Text.Json;
using AuthService.AsyncDataServices;
using AuthService.Commons;
using AuthService.Databases.InitDb;
using AuthService.Extensions;
using AuthService.GrpcServices;
using AuthService.Middlewares;
using AuthService.Services.MailSender;
using AuthService.Settings;
using Microsoft.AspNetCore.Mvc;
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
builder.Services.Configure<GoogleAuthenticationSetting>(builder.Configuration.GetSection("Authentication:Google"));

builder.Services.AddSingleton<IMessagePublisher, MessageBusProvider>();

builder.Services.AddHttpContextAccessor(); // Add IHttpContextAccessor for getting HttpContext in services
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
builder.Services.AddHttpClient();
builder.Services.AddCustomCorsConfig();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Sử dụng camelCase cho tên property của JSON response
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddAuthorization();

builder.Services.AddGrpc();

if (builder.Environment.IsProduction())
{
    builder.WebHost.UseUrls("http://0.0.0.0:80");
}

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // Ngăn chặn response ngay lập tức khi ModelState không hợp lệ, để có thể xử lý lỗi ở Controller
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

var dbInit = app.Services.GetRequiredService<IDbInitializer>();
await dbInit.Initialize();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "auth-service/swagger/{documentName}/swagger.json";
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/auth-service/swagger/v1/swagger.json", "Auth API V1");
        options.RoutePrefix = "auth-service/swagger"; // Prefix cho Swagger UI
        options.InjectStylesheet("/auth-service/swagger/custom-swagger.css"); // Đường dẫn cho file CSS
        options.InjectJavascript("/auth-service/swagger/custom-swagger.js"); // Đường dẫn cho file JavaScript
    });
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors("AllowSpecificOrigin");

app.UseStaticFiles();

// Đặt trước middleware Authentication và Authorization để mới có thể handle response khi không được phép truy cập
app.UseMiddleware<CustomUnauthorizedMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<GrpcAuthServices>();
app.MapControllers();
await app.RunAsync();
