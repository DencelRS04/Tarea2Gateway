using ProductosService.Data;
using ProductosService.Repositories;
using ProductosService.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mongo settings
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("Mongo"));

// DI
builder.Services.AddSingleton<IProductosRepository, ProductosRepository>();
builder.Services.AddScoped<IProductosService, ProductosService.Services.ProductosService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Logs mínimos por request
app.Use(async (ctx, next) =>
{
    var logger = ctx.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("ProductosService");
    var start = DateTime.UtcNow;

    await next();

    var ms = (DateTime.UtcNow - start).TotalMilliseconds;
    logger.LogInformation("[Productos] {Method} {Path} -> {StatusCode} ({Elapsed}ms)",
        ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode, Math.Round(ms));
});

app.MapControllers();
app.Run();