using System.ComponentModel.DataAnnotations;

namespace ProductosService.Dtos;

public class ProductoCreateDto
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string nombre { get; set; } = string.Empty;

    // Se valida en el Service (Precio > 0)
    public decimal precio { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int stock { get; set; }
}