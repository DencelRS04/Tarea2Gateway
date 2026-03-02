using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ClientesService.Dtos;
using ClientesService.Services;

namespace ClientesService.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly IClientesService _service;
    private readonly ILogger<ClientesController> _logger;

    public ClientesController(IClientesService service, ILogger<ClientesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var list = await _service.ListarAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest(new { error = "Id inválido (debe ser ObjectId de 24 caracteres hex)" });

        var cliente = await _service.ObtenerAsync(id);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] ClienteCreateDto dto)
    {
        var (ok, error, cliente) = await _service.CrearAsync(dto);

        if (!ok) return BadRequest(new { error });

        _logger.LogInformation("Cliente registrado: {Id} - {Nombre}", cliente!.id, cliente.nombre);

        return CreatedAtAction(nameof(Obtener), new { id = cliente!.id }, cliente);
    }
}