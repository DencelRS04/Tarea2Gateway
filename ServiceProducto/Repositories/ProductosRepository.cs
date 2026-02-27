using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ProductosService.Data;
using ProductosService.Models;

namespace ProductosService.Repositories;

public class ProductosRepository : IProductosRepository
{
    private readonly IMongoCollection<Producto> _col;

    public ProductosRepository(IOptions<MongoSettings> options)
    {
        var cfg = options.Value;
        var client = new MongoClient(cfg.ConnectionString);
        var db = client.GetDatabase(cfg.Database);
        _col = db.GetCollection<Producto>(cfg.Collection);
    }

    public async Task<List<Producto>> GetAllAsync() =>
        await _col.Find(_ => true).ToListAsync();

    public async Task<Producto?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _)) return null;
        return await _col.Find(p => p.id == id).FirstOrDefaultAsync();
    }

    public async Task<Producto?> GetByNombreAsync(string nombre) =>
        await _col.Find(p => p.nombre == nombre).FirstOrDefaultAsync();

    public async Task<Producto> CreateAsync(Producto producto)
    {
        await _col.InsertOneAsync(producto);
        return producto;
    }

    public async Task<bool> UpdateAsync(string id, Producto producto)
    {
        if (!ObjectId.TryParse(id, out _)) return false;

        producto.id = id;
        var result = await _col.ReplaceOneAsync(p => p.id == id, producto);
        return result.MatchedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _)) return false;

        var result = await _col.DeleteOneAsync(p => p.id == id);
        return result.DeletedCount > 0;
    }
}