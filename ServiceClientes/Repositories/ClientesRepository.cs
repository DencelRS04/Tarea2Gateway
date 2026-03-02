using MongoDB.Driver;
using ClientesService.Data;
using ClientesService.Models;
using Microsoft.Extensions.Options;

namespace ClientesService.Repositories;

public class ClientesRepository : IClientesRepository
{
    private readonly IMongoCollection<Cliente> _collection;

    public ClientesRepository(IOptions<MongoSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.Database);
        _collection = database.GetCollection<Cliente>(settings.Value.Collection);
        
        // Crear índice único para email
        var indexKeys = Builders<Cliente>.IndexKeys.Ascending(c => c.email);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<Cliente>(indexKeys, indexOptions);
        _collection.Indexes.CreateOneAsync(indexModel);
    }

    public async Task<List<Cliente>> ListarAsync() =>
        await _collection.Find(_ => true).SortByDescending(c => c.fechaRegistro).ToListAsync();

    public async Task<Cliente?> ObtenerAsync(string id) =>
        await _collection.Find(c => c.id == id).FirstOrDefaultAsync();

    public async Task<Cliente?> ObtenerPorEmailAsync(string email) =>
        await _collection.Find(c => c.email == email).FirstOrDefaultAsync();

    public async Task<Cliente> CrearAsync(Cliente cliente)
    {
        await _collection.InsertOneAsync(cliente);
        return cliente;
    }
}