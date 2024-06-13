using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStoreApi.Services
{
    public class PeliculasService
    {
        private readonly IMongoCollection<Pelicula> _peliculasCollection;

        public PeliculasService(
            IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _peliculasCollection = mongoDatabase.GetCollection<Pelicula>(
                bookStoreDatabaseSettings.Value.PeliculasCollectionName);
        }

        public async Task<List<Pelicula>> GetAsync() =>
            await _peliculasCollection.Find(_ => true).ToListAsync();

        public async Task<Pelicula?> GetAsync(string id) =>
            await _peliculasCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Pelicula newPelicula) =>
            await _peliculasCollection.InsertOneAsync(newPelicula);

        public async Task UpdateAsync(string id, Pelicula updatedPelicula) =>
            await _peliculasCollection.ReplaceOneAsync(x => x.Id == id, updatedPelicula);

        public async Task RemoveAsync(string id) =>
            await _peliculasCollection.DeleteOneAsync(x => x.Id == id);
    }
}
