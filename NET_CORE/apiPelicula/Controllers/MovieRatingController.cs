using Microsoft.AspNetCore.Mvc;
using BookStoreApi.Models;
using BookStoreApi.Services;


namespace BookStoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieRatingController : ControllerBase
    {
        private readonly MoviesRatingService _moviesRatingService;
        private readonly RedisService _redisService;

        public MovieRatingController(MoviesRatingService moviesRatingService, RedisService redisService)
        {
            _moviesRatingService = moviesRatingService;
            _redisService = redisService;
        }

        [HttpGet]
        public async Task<List<MovieRating>> Get() => await _moviesRatingService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<MovieRating>> Get(string id)
        {
            var movieRating = await _moviesRatingService.GetAsync(id);
            if (movieRating is null) return NotFound();
            return movieRating;
        }

        [HttpPost]
        public async Task<IActionResult> Post(MovieRating newMovieRating)
        {
            await _moviesRatingService.CreateAsync(newMovieRating);
            return CreatedAtAction(nameof(Get), new { id = newMovieRating.Id }, newMovieRating);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, MovieRating updatedMovieRating)
        {
            var movieRating = await _moviesRatingService.GetAsync(id);
            if (movieRating is null) return NotFound();
            updatedMovieRating.Id = movieRating.Id;
            await _moviesRatingService.UpdateAsync(id, updatedMovieRating);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var movieRating = await _moviesRatingService.GetAsync(id);
            if (movieRating is null) return NotFound();
            await _moviesRatingService.RemoveAsync(id);
            return NoContent();
        }

        [HttpPost("transfer")]
        public IActionResult TransferFromRedisToMongo()
        {
            _ = _redisService.TransferMoviesRatingToMongoDB(); 
            return Ok();
        }

    }
}