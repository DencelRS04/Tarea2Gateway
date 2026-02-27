using ProductosService.Models;

namespace ProductosService.Repositories;

public interface IProductosRepository
{
    Task<List<Producto>> GetAllAsync();
    Task<Producto?> GetByIdAsync(string id);
    Task<Producto?> GetByNombreAsync(string nombre);
    Task<Producto> CreateAsync(Producto producto);
    Task<bool> UpdateAsync(string id, Producto producto);
    Task<bool> DeleteAsync(string id);
}