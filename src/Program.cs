using Sphera.API.Configurations;
using Sphera.API.External;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Nome da policy (pode ser qualquer string)
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// 🔹 Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:8080") // endereço do seu front
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddIoC(builder.Configuration);
builder.Services.ConfigureEndpoints();

builder.Services.AddOpenApi();

var app = builder.Build();
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();
app.MapControllers();
app.UpdateMigrations();

app.Run();