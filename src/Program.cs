using Sphera.API.Configurations;
using Sphera.API.External;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddIoC(builder.Configuration);
builder.Services.ConfigureEndpoints();

builder.Services.AddOpenApi();

#if DEBUG
builder.Services.AddCors(options =>
{
    options.AddPolicy("Localhost8080", policy =>
    {
        policy.WithOrigins("http://localhost:8080")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
#endif

var app = builder.Build();
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

#if DEBUG
app.UseCors("Localhost8080");
#endif

// #if !DEBUG
// app.UseMiddleware<LicenseMiddleware>();
// #endif

app.UseAuthorization();
app.MapControllers();
app.UpdateMigrations();

app.Run();