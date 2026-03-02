using ClientesService.Models;

namespace ClientesService.Repositories;

public interface IClientesRepository
{
    Task<List<Cliente>> ListarAsync();
    Task<Cliente?> ObtenerAsync(string id);
    Task<Cliente?> ObtenerPorEmailAsync(string email);
    Task<Cliente> CrearAsync(Cliente cliente);
}