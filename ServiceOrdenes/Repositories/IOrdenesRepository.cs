using OrdenesService.Models;

namespace OrdenesService.Repositories;

public interface IOrdenesRepository
{
    Task<List<Orden>> ListarAsync();
    Task<Orden?> ObtenerAsync(string id);
    Task<Orden> CrearAsync(Orden orden);
    Task<List<Orden>> ListarPorClienteAsync(string clienteId);
}