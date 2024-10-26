using CourseManagementService.Common;
using CourseManagementService.Database.InitDb;
using CourseManagementService.Extensions;
using CourseManagementService.Middlewares;
using CourseManagementService.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.UseCustomLog(Constants.SERVICE_NAME);
builder.AddAutoFact();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();

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

app.UseRouting();
app.UseCors("AllowSpecificOrigin");

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CustomUnauthorizedMiddleware>();
app.MapControllers();
await app.RunAsync();
