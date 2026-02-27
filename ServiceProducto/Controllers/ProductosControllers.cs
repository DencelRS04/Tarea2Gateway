using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProductosService.Dtos;
using ProductosService.Services;

namespace ProductosService.Controllers;

[ApiController]
[Route("api/productos")]
public class ProductosController : ControllerBase
{
    private readonly IProductosService _service;
    private readonly ILogger<ProductosController> _logger;

    public ProductosController(IProductosService service, ILogger<ProductosController> logger)
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

        var p = await _service.ObtenerAsync(id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] ProductoCreateDto dto)
    {
        var (ok, error, producto) = await _service.CrearAsync(dto);

        if (!ok) return BadRequest(new { error });

        _logger.LogInformation("Producto creado: {Id} - {Nombre}", producto!.id, producto.nombre);

        return CreatedAtAction(nameof(Obtener), new { id = producto!.id }, producto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(string id, [FromBody] ProductoUpdateDto dto)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest(new { error = "Id inválido (debe ser ObjectId de 24 caracteres hex)" });
        var (ok, error) = await _service.ActualizarAsync(id, dto);

        if (!ok)
        {
            if (error == "Producto no existe") return NotFound(new { error });
            return BadRequest(new { error });
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return BadRequest(new { error = "Id inválido (debe ser ObjectId de 24 caracteres hex)" });

        var ok = await _service.EliminarAsync(id);
        return ok ? NoContent() : NotFound();
    }
}