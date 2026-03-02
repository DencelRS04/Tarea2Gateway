using ClientesService.Data;
using ClientesService.Repositories;
using ClientesService.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración MongoDB
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

// Registrar servicios
builder.Services.AddSingleton<IClientesRepository, ClientesRepository>();
builder.Services.AddScoped<IClientesService, ClientesAppService>(); 

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
