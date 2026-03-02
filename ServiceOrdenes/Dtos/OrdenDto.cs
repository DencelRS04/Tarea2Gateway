namespace OrdenesService.Dtos;

public class OrdenDto
{
    public string id { get; set; } = string.Empty;
    public string clienteId { get; set; } = string.Empty;
    public List<OrdenItemDto> items { get; set; } = new();
    public decimal total { get; set; }
    public string estado { get; set; } = string.Empty;
    public DateTime fechaCreacion { get; set; }
}

public class OrdenItemDto
{
    public string productoId { get; set; } = string.Empty;
    public string nombreProducto { get; set; } = string.Empty;
    public decimal precioUnitario { get; set; }
    public int cantidad { get; set; }
    public decimal subtotal { get; set; }
}