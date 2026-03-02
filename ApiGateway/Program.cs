using System.Net;
using System.Net.Http.Json;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// YARP
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// HttpClient para llamadas inter-servicio desde el Gateway (agregación)
builder.Services.AddHttpClient("Ordenes", c =>
{
    c.BaseAddress = new Uri("http://localhost:5002/");
});
builder.Services.AddHttpClient("Productos", c =>
{
    c.BaseAddress = new Uri("https://localhost:7172/");
});

var app = builder.Build();

// Logs mínimos del Gateway (observabilidad)
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("ApiGateway");
    logger.LogInformation("[Gateway] {Method} {Path}", context.Request.Method, context.Request.Path);

    await next();

    logger.LogInformation("[Gateway] Response {StatusCode}", context.Response.StatusCode);
});

// ✅ Agregación: /api/resumen-orden/{id}
// Trae orden desde MS Órdenes + productos desde MS Productos
app.MapGet("/api/resumen-orden/{id}", async (string id, IHttpClientFactory httpFactory) =>
{
    var ordenesHttp = httpFactory.CreateClient("Ordenes");
    var productosHttp = httpFactory.CreateClient("Productos");

    // 1) Traer la orden
    var ordenResp = await ordenesHttp.GetAsync($"api/ordenes/{id}");
    if (ordenResp.StatusCode == HttpStatusCode.NotFound)
        return Results.NotFound(new { error = "Orden no existe" });

    if (!ordenResp.IsSuccessStatusCode)
        return Results.Problem("No se pudo consultar el microservicio de Órdenes.");

    var orden = await ordenResp.Content.ReadFromJsonAsync<OrdenDto>();
    if (orden is null)
        return Results.Problem("Respuesta inválida del microservicio de Órdenes.");

    // 2) Traer detalle de cada producto (en paralelo)
    var tasks = orden.Items.Select(async item =>
    {
        var prodResp = await productosHttp.GetAsync($"api/productos/{item.ProductoId}");
        if (!prodResp.IsSuccessStatusCode)
        {
            return new ProductoDetalleDto(
                item.ProductoId,
                "NO DISPONIBLE",
                0,
                item.Cantidad
            );
        }

        var p = await prodResp.Content.ReadFromJsonAsync<ProductoDto>();
        if (p is null)
        {
            return new ProductoDetalleDto(
                item.ProductoId,
                "NO DISPONIBLE",
                0,
                item.Cantidad
            );
        }

        return new ProductoDetalleDto(
            item.ProductoId,
            p.nombre ?? "SIN NOMBRE",
            p.precio,
            item.Cantidad
        );
    });

    var productos = await Task.WhenAll(tasks);

    // 3) Respuesta agregada
    return Results.Ok(new
    {
        orden = new
        {
            orden.Id,
            orden.ClienteId,
            orden.Estado,
            orden.Fecha,
            items = orden.Items
        },
        productos,
        total = productos.Sum(x => x.Precio * x.Cantidad)
    });
});

app.MapReverseProxy();
app.Run();


// ===== DTOs mínimos para leer JSON =====
// Ajusta nombres si tus microservicios devuelven campos distintos.

record OrdenDto(
    string Id,
    string ClienteId,
    string Estado,
    DateTime Fecha,
    List<ItemDto> Items
);

record ItemDto(
    string ProductoId,
    int Cantidad
);

record ProductoDto(
    string id,
    string nombre,
    decimal precio,
    int stock
);

record ProductoDetalleDto(
    string ProductoId,
    string Nombre,
    decimal Precio,
    int Cantidad
);