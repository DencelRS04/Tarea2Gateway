using ProductosService.Dtos;
using ProductosService.Models;

namespace ProductosService.Services;

public interface IProductosService
{
    Task<List<Producto>> ListarAsync();
    Task<Producto?> ObtenerAsync(string id);
    Task<(bool ok, string? error, Producto? producto)> CrearAsync(ProductoCreateDto dto);
    Task<(bool ok, string? error)> ActualizarAsync(string id, ProductoUpdateDto dto);
    Task<bool> EliminarAsync(string id);
}