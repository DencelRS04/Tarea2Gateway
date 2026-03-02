using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrdenesService.Models;

public class Orden
{
    [BsonId]
    [BsonRepresentationAttribute(BsonType.ObjectId)]
    public string? id { get; set; }

    [BsonElement("clienteId")]
    public string clienteId { get; set; } = string.Empty;

    [BsonElement("items")]
    public List<OrdenItem> items { get; set; } = new();

    [BsonElement("total")]
    public decimal total { get; set; }

    [BsonElement("estado")]
    public string estado { get; set; } = "Pendiente"; // Pendiente, Confirmada, Cancelada

    [BsonElement("fechaCreacion")]
    public DateTime fechaCreacion { get; set; } = DateTime.UtcNow;
}

public class OrdenItem
{
    [BsonElement("productoId")]
    public string productoId { get; set; } = string.Empty;

    [BsonElement("nombreProducto")]
    public string nombreProducto { get; set; } = string.Empty;

    [BsonElement("precioUnitario")]
    public decimal precioUnitario { get; set; }

    [BsonElement("cantidad")]
    public int cantidad { get; set; }

    [BsonElement("subtotal")]
    public decimal subtotal { get; set; }
}