using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeliculasController : ControllerBase
    {
        private readonly PeliculasService _peliculasService;

        public PeliculasController(PeliculasService peliculasService) =>
            _peliculasService = peliculasService;

        [HttpGet]
        public async Task<List<Pelicula>> Get() =>
            await _peliculasService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Pelicula>> Get(string id)
        {
            var pelicula = await _peliculasService.GetAsync(id);

            if (pelicula is null)
            {
                return NotFound();
            }

            return pelicula;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Pelicula newPelicula)
        {
            await _peliculasService.CreateAsync(newPelicula);

            return CreatedAtAction(nameof(Get), new { id = newPelicula.Id }, newPelicula);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Pelicula updatedPelicula)
        {
            var pelicula = await _peliculasService.GetAsync(id);

            if (pelicula is null)
            {
                return NotFound();
            }

            updatedPelicula.Id = pelicula.Id;

            await _peliculasService.UpdateAsync(id, updatedPelicula);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var pelicula = await _peliculasService.GetAsync(id);

            if (pelicula is null)
            {
                return NotFound();
            }

            await _peliculasService.RemoveAsync(id);

            return NoContent();
        }
    }
}
