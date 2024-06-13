using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeneroController : ControllerBase
    {
        private readonly GenerosService _generosService;

        public GeneroController(GenerosService generosService)
        {
            _generosService = generosService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Genero>>> GetGeneros()
        {
            var generos = await _generosService.GetAsync();
            return Ok(generos);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Genero>> GetGenero(string id)
        {
            var genero = await _generosService.GetAsync(id);
            if (genero == null)
            {
                return NotFound();
            }
            return genero;
        }

        [HttpPost]
        public async Task<ActionResult<Genero>> CreateGenero(Genero genero)
        {
            await _generosService.CreateAsync(genero);
            return CreatedAtAction(nameof(GetGenero), new { id = genero.Id }, genero);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdateGenero(string id, Genero genero)
        {
            var existingGenero = await _generosService.GetAsync(id);
            if (existingGenero == null)
            {
                return NotFound();
            }
            await _generosService.UpdateAsync(id, genero);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteGenero(string id)
        {
            var genero = await _generosService.GetAsync(id);
            if (genero == null)
            {
                return NotFound();
            }
            await _generosService.RemoveAsync(id);
            return NoContent();
        }
    }
}
