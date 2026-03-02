using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OrdenesService.Dtos;
using OrdenesService.Services;

namespace OrdenesService.Controllers;

[ApiController]
[Route("api/ordenes")]
public class OrdenesController : ControllerBase
{
    private readonly IOrdenesService _service;
    private readonly ILogger<OrdenesController> _logger;

    public OrdenesController(IOrdenesService service, ILogger<OrdenesController> logger)
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

        var orden = await _service.ObtenerAsync(id);
        return orden is null ? NotFound() : Ok(orden);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] OrdenCreateDto dto)
    {
        var (ok, error, orden) = await _service.CrearAsync(dto);

        if (!ok) return BadRequest(new { error });

        _logger.LogInformation("Orden creada: {Id} - Total: {Total}", orden!.id, orden.total);

        return CreatedAtAction(nameof(Obtener), new { id = orden!.id }, orden);
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<IActionResult> ListarPorCliente(string clienteId)
    {
        if (!ObjectId.TryParse(clienteId, out _))
            return BadRequest(new { error = "ClienteId inválido" });

        var ordenes = await _service.ListarPorClienteAsync(clienteId);
        return Ok(ordenes);
    }
}