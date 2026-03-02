using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClientesService.Models;

public class Cliente
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? id { get; set; }

    [BsonElement("nombre")]
    public string nombre { get; set; } = string.Empty;

    [BsonElement("email")]
    public string email { get; set; } = string.Empty;

    [BsonElement("telefono")]
    public string? telefono { get; set; }

    [BsonElement("fechaRegistro")]
    public DateTime fechaRegistro { get; set; } = DateTime.UtcNow;
}