namespace ClientesService.Dtos;

public class ClienteDto
{
    public string id { get; set; } = string.Empty;
    public string nombre { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string? telefono { get; set; }
    public DateTime fechaRegistro { get; set; }
}