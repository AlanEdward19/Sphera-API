using Sphera.API.Clients;
using Sphera.API.External;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureClientsRelatedDependencies();
builder.Services.AddExernal(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseAuthorization();
app.MapControllers();
app.Run();