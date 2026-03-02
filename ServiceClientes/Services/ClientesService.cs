using MongoDB.Bson;
using MongoDB.Driver;
using ClientesService.Dtos;
using ClientesService.Models;
using ClientesService.Repositories;

namespace ClientesService.Services;

public class ClientesAppService : IClientesService 
{
    private readonly IClientesRepository _repository;
    private readonly ILogger<ClientesAppService> _logger; 

    public ClientesAppService(IClientesRepository repository, ILogger<ClientesAppService> logger) 
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<ClienteDto>> ListarAsync()
    {
        var clientes = await _repository.ListarAsync();
        return clientes.Select(MapToDto).ToList();
    }

    public async Task<ClienteDto?> ObtenerAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        var cliente = await _repository.ObtenerAsync(id);
        return cliente is null ? null : MapToDto(cliente);
    }

    public async Task<(bool ok, string? error, ClienteDto? cliente)> CrearAsync(ClienteCreateDto dto)
    {
        // Validar email único
        var existe = await _repository.ObtenerPorEmailAsync(dto.email);
        if (existe is not null)
            return (false, "El email ya está registrado.", null);

        var cliente = new Cliente
        {
            nombre = dto.nombre,
            email = dto.email,
            telefono = dto.telefono,
            fechaRegistro = DateTime.UtcNow
        };

        try
        {
            var nuevoCliente = await _repository.CrearAsync(cliente);
            _logger.LogInformation("Cliente registrado: {Id} - {Email}", nuevoCliente.id, nuevoCliente.email);
            return (true, null, MapToDto(nuevoCliente));
        }
        catch (MongoException ex)
        {
            _logger.LogError(ex, "Error al registrar cliente");
            return (false, "Error al registrar el cliente.", null);
        }
    }

    private static ClienteDto MapToDto(Cliente cliente) => new()
    {
        id = cliente.id ?? string.Empty,
        nombre = cliente.nombre,
        email = cliente.email,
        telefono = cliente.telefono,
        fechaRegistro = cliente.fechaRegistro
    };
}