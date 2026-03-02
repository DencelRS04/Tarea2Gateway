using System.ComponentModel.DataAnnotations;

namespace ClientesService.Dtos;

public class ClienteCreateDto
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido.")]
    [EmailAddress(ErrorMessage = "Email inválido.")]
    public string email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Teléfono inválido.")]
    public string? telefono { get; set; }
}