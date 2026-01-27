using DataAccess;
using DataAccess.Seeds;
using Microsoft.AspNetCore.HttpOverrides;
using WebApi.Extensions;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddDataAnnotationsLocalization().AddViewLocalization();
builder.Services.AddHttpContextAccessor();

// Swagger
builder.Services.AddSwaggerServices();

// Database
builder.Services.AddDatabaseServices(builder.Configuration);

// Settings
builder.Services.AddSettingsServices(builder.Configuration);

// Security
builder.Services.AddSecurityServices(builder.Configuration);

// Hosts
builder.Services.AddHostServices();

// Loggers
builder.Services.AddLoggerServices();

// Repositories & Unit of Work
builder.Services.AddDataAccessServices();

// Services
builder.Services.AddApplicationServices();

// Web (CORS, Localization)
builder.Services.AddWebServices(builder.Configuration);

var app = builder.Build();

// Middlewares
app.UseMiddleware<ApiLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseRequestLocalization();

app.UseForwardedHeaders(
    new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    }
);

app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Comment this line to prevent automatic create data
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MyBlogContext>();
    Seeder.Seed(db);
}

app.UseCors("_myAllowSpecificOrigins");

app.UseAuthentication();

app.UseMiddleware<UserValidationMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
