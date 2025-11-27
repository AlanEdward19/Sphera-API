using Sphera.API.Configurations;
using Sphera.API.External;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddIoC(builder.Configuration);
builder.Services.ConfigureEndpoints();

builder.Services.AddOpenApi();

var app = builder.Build();
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseAuthorization();
app.MapControllers();
app.UpdateMigrations();

app.Run();