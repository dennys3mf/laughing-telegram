using Microsoft.Extensions.Options;
using MongoDB.Driver;
using BookStoreApi.Models;

namespace BookStoreApi.Services
{
    public class MoviesRatingService
    {
        private readonly IMongoCollection<MovieRating> _moviesRatingCollection;

        public MoviesRatingService(IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(bookStoreDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName);
            _moviesRatingCollection = mongoDatabase.GetCollection<MovieRating>(bookStoreDatabaseSettings.Value.MovieRatingCollectionName);
        }

        public async Task<List<MovieRating>> GetAsync() =>
            await _moviesRatingCollection.Find(_ => true).ToListAsync();

        public async Task<MovieRating?> GetAsync(string id) => 
            await _moviesRatingCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(MovieRating newMovieRating) => 
            await _moviesRatingCollection.InsertOneAsync(newMovieRating);

        public async Task UpdateAsync(string id, MovieRating updatedMovieRating) => 
            await _moviesRatingCollection.ReplaceOneAsync(x => x.Id == id, updatedMovieRating);

        public async Task RemoveAsync(string id) => await _moviesRatingCollection.DeleteOneAsync(x => x.Id == id);
    }
}