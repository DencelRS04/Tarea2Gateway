using MongoDB.Bson;
using OrdenesService.Dtos;
using OrdenesService.Models;
using OrdenesService.Repositories;

namespace OrdenesService.Services;

public class OrdenesAppService : IOrdenesService 
{
    private readonly IOrdenesRepository _repository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrdenesAppService> _logger; 

    public OrdenesAppService( 
        IOrdenesRepository repository,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<OrdenesAppService> logger) 
    {
        _repository = repository;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<List<OrdenDto>> ListarAsync()
    {
        var ordenes = await _repository.ListarAsync();
        return ordenes.Select(MapToDto).ToList();
    }

    public async Task<OrdenDto?> ObtenerAsync(string id)
    {
        var orden = await _repository.ObtenerAsync(id);
        return orden is null ? null : MapToDto(orden);
    }

    public async Task<(bool ok, string? error, OrdenDto? orden)> CrearAsync(OrdenCreateDto dto)
    {
        // Validar que el clienteId sea un ObjectId válido
        if (!ObjectId.TryParse(dto.clienteId, out _))
            return (false, "ClienteId inválido", null);

        var orden = new Orden
        {
            clienteId = dto.clienteId,
            items = new List<OrdenItem>()
        };

        // Consultar productos al Servicio A (comunicación inter-servicio)
        var productosUrl = _configuration["ServiciosUrls:Productos"] ?? "http://localhost:5001/api/productos";
        var httpClient = _httpClientFactory.CreateClient();

        foreach (var item in dto.items)
        {
            try
            {
                // Validar que productoId sea válido
                if (!ObjectId.TryParse(item.productoId, out _))
                    return (false, $"ProductoId inválido: {item.productoId}", null);

                // Consultar producto
                var response = await httpClient.GetAsync($"{productosUrl}/{item.productoId}");
                
                if (!response.IsSuccessStatusCode)
                    return (false, $"Producto no encontrado: {item.productoId}", null);

                var producto = await response.Content.ReadFromJsonAsync<ProductoDto>();
                
                if (producto is null)
                    return (false, $"Error al obtener producto: {item.productoId}", null);

                // Validar stock
                if (producto.stock < item.cantidad)
                    return (false, $"Stock insuficiente para {producto.nombre}. Disponible: {producto.stock}", null);

                var ordenItem = new OrdenItem
                {
                    productoId = item.productoId,
                    nombreProducto = producto.nombre,
                    precioUnitario = producto.precio,
                    cantidad = item.cantidad,
                    subtotal = producto.precio * item.cantidad
                };

                orden.items.Add(ordenItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar producto {ProductoId}", item.productoId);
                return (false, $"Error al validar producto: {item.productoId}", null);
            }
        }

        // Calcular total
        orden.total = orden.items.Sum(i => i.subtotal);

        // Guardar orden
        var nuevaOrden = await _repository.CrearAsync(orden);
        return (true, null, MapToDto(nuevaOrden));
    }

    public async Task<List<OrdenDto>> ListarPorClienteAsync(string clienteId)
    {
        var ordenes = await _repository.ListarPorClienteAsync(clienteId);
        return ordenes.Select(MapToDto).ToList();
    }

    private static OrdenDto MapToDto(Orden orden) => new()
    {
        id = orden.id ?? string.Empty,
        clienteId = orden.clienteId,
        items = orden.items.Select(i => new OrdenItemDto
        {
            productoId = i.productoId,
            nombreProducto = i.nombreProducto,
            precioUnitario = i.precioUnitario,
            cantidad = i.cantidad,
            subtotal = i.subtotal
        }).ToList(),
        total = orden.total,
        estado = orden.estado,
        fechaCreacion = orden.fechaCreacion
    };
}

// DTO auxiliar para deserializar la respuesta del servicio de productos
public class ProductoDto
{
    public string id { get; set; } = string.Empty;
    public string nombre { get; set; } = string.Empty;
    public decimal precio { get; set; }
    public int stock { get; set; }
}