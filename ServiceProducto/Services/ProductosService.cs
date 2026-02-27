using ProductosService.Dtos;
using ProductosService.Models;
using ProductosService.Repositories;

namespace ProductosService.Services;

public class ProductosService : IProductosService
{
    private readonly IProductosRepository _repo;

    public ProductosService(IProductosRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Producto>> ListarAsync() => _repo.GetAllAsync();

    public Task<Producto?> ObtenerAsync(string id) => _repo.GetByIdAsync(id);

    public async Task<(bool ok, string? error, Producto? producto)> CrearAsync(ProductoCreateDto dto)
    {
        var error = Validar(dto.nombre, dto.precio, dto.stock);
        if (error != null) return (false, error, null);

        var p = new Producto
        {
            nombre = dto.nombre.Trim(),
            precio = dto.precio,
            stock = dto.stock
        };

        await _repo.CreateAsync(p);
        return (true, null, p);
    }

    public async Task<(bool ok, string? error)> ActualizarAsync(string id, ProductoUpdateDto dto)
    {
        var error = Validar(dto.nombre, dto.precio, dto.stock);
        if (error != null) return (false, error);

        var existe = await _repo.GetByIdAsync(id);
        if (existe is null) return (false, "Producto no existe");

        var actualizado = new Producto
        {
            id = id,
            nombre = dto.nombre.Trim(),
            precio = dto.precio,
            stock = dto.stock
        };

        var ok = await _repo.UpdateAsync(id, actualizado);
        return ok ? (true, null) : (false, "No se pudo actualizar");
    }

    public Task<bool> EliminarAsync(string id) => _repo.DeleteAsync(id);

    private static string? Validar(string nombre, decimal precio, int stock)
    {
        if (string.IsNullOrWhiteSpace(nombre)) return "Nombre es requerido";
        if (precio <= 0) return "Precio debe ser mayor a 0";
        if (stock < 0) return "Stock no puede ser negativo";
        return null;
    }
}