using ClientesService.Dtos;

namespace ClientesService.Services;

public interface IClientesService
{
    Task<List<ClienteDto>> ListarAsync();
    Task<ClienteDto?> ObtenerAsync(string id);
    Task<(bool ok, string? error, ClienteDto? cliente)> CrearAsync(ClienteCreateDto dto);
}