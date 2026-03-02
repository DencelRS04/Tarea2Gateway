using System.ComponentModel.DataAnnotations;

namespace OrdenesService.Dtos;

public class OrdenCreateDto
{
    [Required(ErrorMessage = "El clienteId es requerido.")]
    public string clienteId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe incluir al menos un item.")]
    [MinLength(1, ErrorMessage = "La orden debe tener al menos un producto.")]
    public List<OrdenItemCreateDto> items { get; set; } = new();
}

public class OrdenItemCreateDto
{
    [Required(ErrorMessage = "El productoId es requerido.")]
    public string productoId { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public int cantidad { get; set; }
}