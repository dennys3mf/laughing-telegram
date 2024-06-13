using BookStoreApi.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BookStoreApi.Services
{
    public class RedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly MoviesRatingService _moviesRatingService;
        private Timer _transferTimer;
        private readonly TimeSpan _transferInterval = TimeSpan.FromSeconds(2); // Cambio en el intervalo



        public RedisService(IConnectionMultiplexer redis, MoviesRatingService moviesRatingService)
        {
            _redis = redis;
            _moviesRatingService = moviesRatingService;
            Console.WriteLine("Conexión a Redis establecida correctamente.");

            _transferTimer = new Timer(async _ =>
               {
                   await TransferMoviesRatingToMongoDB();
               }, null, TimeSpan.Zero, _transferInterval);

        }

        // imprime en consola los datos de redis
        private async Task PrintMovieRatings()
        {
            var movieRatings = await GetMoviesRatingFromRedis();
            foreach (var rating in movieRatings)
            {
                Console.WriteLine($"UsuarioId: {rating.UsuarioId}, PeliculaId: {rating.PeliculaId}, ViewingTime: {rating.ViewingTime}");
            }
        }

        public async Task<List<MovieRating>> GetMoviesRatingFromRedis()
        {
            var db = _redis.GetDatabase();
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var movieKeys = server.Keys(pattern: "id_del_usuario:*");

            var moviesRating = new List<MovieRating>();

            foreach (var key in movieKeys)
            {
                var movieRatingData = await db.HashGetAllAsync(key);
                var movieRatingDict = movieRatingData.ToStringDictionary();

                if (movieRatingDict != null && movieRatingDict.Count > 0)
                {
                    var usuarioId = key.ToString().Split(':')[1]; // Extraer el ID del usuario de la clave

                    // Obtener el ID de la película del diccionario de datos
                    var peliculaId = movieRatingDict.ContainsKey("ID_pelicula") ? movieRatingDict["ID_pelicula"] : "No disponible";

                    var movieRating = new MovieRating
                    {
                        UsuarioId = usuarioId,
                        PeliculaId = peliculaId,
                        ViewingTime = double.Parse(movieRatingDict["tiempo_visto"])
                    };
                    moviesRating.Add(movieRating);
                }
            }

            return moviesRating;
        }

        public async Task<List<MovieRating>> TransferMoviesRatingToMongoDB()
        {
            var movieRatings = await GetMoviesRatingFromRedis();
            foreach (var rating in movieRatings)
            {
                await _moviesRatingService.CreateAsync(rating);
            }

            // Eliminar los datos de Redis después de transferirlos a MongoDB
            await RemoveMoviesRatingFromRedis(movieRatings);

            return movieRatings;
        }

        private async Task RemoveMoviesRatingFromRedis(List<MovieRating> movieRatings)
        {
            var db = _redis.GetDatabase();
            foreach (var rating in movieRatings)
            {
                var key = $"id_del_usuario:{rating.UsuarioId}";
                await db.KeyDeleteAsync(key);
            }
        }

    }
}