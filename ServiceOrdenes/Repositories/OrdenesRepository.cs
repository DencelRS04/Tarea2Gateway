using MongoDB.Driver;
using OrdenesService.Data;
using OrdenesService.Models;
using Microsoft.Extensions.Options;

namespace OrdenesService.Repositories;

public class OrdenesRepository : IOrdenesRepository
{
    private readonly IMongoCollection<Orden> _collection;

    public OrdenesRepository(IOptions<MongoSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.Database);
        _collection = database.GetCollection<Orden>(settings.Value.Collection);
    }

    public async Task<List<Orden>> ListarAsync() =>
        await _collection.Find(_ => true).SortByDescending(o => o.fechaCreacion).ToListAsync();

    public async Task<Orden?> ObtenerAsync(string id) =>
        await _collection.Find(o => o.id == id).FirstOrDefaultAsync();

    public async Task<Orden> CrearAsync(Orden orden)
    {
        await _collection.InsertOneAsync(orden);
        return orden;
    }

    public async Task<List<Orden>> ListarPorClienteAsync(string clienteId) =>
        await _collection.Find(o => o.clienteId == clienteId)
            .SortByDescending(o => o.fechaCreacion)
            .ToListAsync();
}