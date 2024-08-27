using System.Reflection.Metadata;
using AuthService.Commons;
using AuthService.Databases;
using AuthService.Databases.InitDb;
using AuthService.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.UseCustomLog(Constants.SERVICE_NAME);
builder.AddAutoFact();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services
// builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
// builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
// builder.Services.AddGrpc();
builder.Services.AddTransient<IDbInitializer, DbInitializer>();
builder.Services.AddDataContext(builder.Configuration);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthService", Version = "v1" });
});

var app = builder.Build();

var dbInit = app.Services.GetRequiredService<IDbInitializer>();
if (app.Environment.IsProduction())
{
    dbInit.Migrate();
}
dbInit.Initialize();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
// app.MapGrpcService<GrpcPlatformService>();

app.MapGet("/protos/platforms.proto", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

app.Run();
