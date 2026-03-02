using OrdenesService.Data;
using OrdenesService.Repositories;
using OrdenesService.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración MongoDB
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

// Registrar servicios
builder.Services.AddSingleton<IOrdenesRepository, OrdenesRepository>();
builder.Services.AddScoped<IOrdenesService, OrdenesAppService>();

// HttpClient para comunicación inter-servicio
builder.Services.AddHttpClient();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
