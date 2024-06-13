using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStoreApi.Services
{
    public class GenerosService
    {
        private readonly IMongoCollection<Genero> _generosCollection;

        public GenerosService(IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(bookStoreDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName);
            _generosCollection = mongoDatabase.GetCollection<Genero>(bookStoreDatabaseSettings.Value.GenerosCollectionName);
        }

        public async Task<List<Genero>> GetAsync() =>
            await _generosCollection.Find(_ => true).ToListAsync();

        public async Task<Genero> GetAsync(string id) =>
            await _generosCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Genero genero) =>
            await _generosCollection.InsertOneAsync(genero);

        public async Task UpdateAsync(string id, Genero genero) =>
            await _generosCollection.ReplaceOneAsync(x => x.Id == id, genero);

        public async Task RemoveAsync(string id) =>
            await _generosCollection.DeleteOneAsync(x => x.Id == id);
    }
}
