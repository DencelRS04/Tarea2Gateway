using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductosService.Models;

public class Producto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    [BsonElement("nombre")]
    public string nombre { get; set; } = string.Empty;

    [BsonElement("precio")]
    public decimal precio { get; set; }

    [BsonElement("stock")]
    public int stock { get; set; }
}