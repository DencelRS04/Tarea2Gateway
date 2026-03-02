using OrdenesService.Dtos;

namespace OrdenesService.Services;

public interface IOrdenesService
{
    Task<List<OrdenDto>> ListarAsync();
    Task<OrdenDto?> ObtenerAsync(string id);
    Task<(bool ok, string? error, OrdenDto? orden)> CrearAsync(OrdenCreateDto dto);
    Task<List<OrdenDto>> ListarPorClienteAsync(string clienteId);
}