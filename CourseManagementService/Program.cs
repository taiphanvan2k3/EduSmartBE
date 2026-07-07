using System.Text.Json;
using CourseManagementService.Common;
using CourseManagementService.Database.InitDb;
using CourseManagementService.Extensions;
using CourseManagementService.GrpcServices;
using CourseManagementService.Hubs;
using CourseManagementService.Middlewares;
using CourseManagementService.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.UseCustomLog(Constants.SERVICE_NAME);
builder.AddAutoFact();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();

builder.Services.AddMinioSettings(builder.Configuration);
builder.Services.Configure<CloudinarySetting>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddHttpContextAccessor(); // Add IHttpContextAccessor for getting HttpContext in services
builder.Services.AddDataContext(builder.Configuration);
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

// Tránh việc tạo nhiều kết nối tới Redis khi mỗi lần cần sử dụng
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>().GetConnectionString("RedisConnection");
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddCustomHostedServices();
builder.Services.AddGemini(builder.Configuration);

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
        // Sử dụng camelCase cho tên property của JSON response -> cho việc tạo camelCase cho example của ProduceResponse
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    })
    .AddNewtonsoftJson(options =>
    {
        // NewtonsoftJSON mặc định dùng PascalCase cho tên property của JSON response
        // nên cần set lại để sử dụng camelCase
        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
    });

builder.Services.AddAuthorization();
builder.Services.AddGrpc();

// Add SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

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
        options.RouteTemplate = "course-service/swagger/{documentName}/swagger.json";
    });


    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/course-service/swagger/v1/swagger.json", "Course API V1");
        options.RoutePrefix = "course-service/swagger"; // Prefix cho Swagger UI
        options.InjectStylesheet("/course-service/swagger/custom-swagger.css"); // Đường dẫn cho file CSS
        options.InjectJavascript("/course-service/swagger/custom-swagger.js"); // Đường dẫn cho file JavaScript
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

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapGrpcService<GrpcCourseService>();
app.MapHub<NotificationHub>("/course-service/hub/notification");

app.MapControllers();
await app.RunAsync();
