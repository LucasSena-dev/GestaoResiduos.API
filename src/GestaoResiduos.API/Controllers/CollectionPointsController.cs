using System.Collections.Generic;
using System.Threading.Tasks;
using GestaoResiduos.API.Services;
using GestaoResiduos.API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestaoResiduos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollectionPointsController : ControllerBase
    {
        private readonly ICollectionPointService _collectionPointService;
        
        public CollectionPointsController(ICollectionPointService collectionPointService)
        {
            _collectionPointService = collectionPointService;
        }
        
        /// <summary>
        /// Obtém todos os pontos de coleta com paginação
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponse<CollectionPointViewModel>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var points = await _collectionPointService.GetAllAsync(page, pageSize);
            return Ok(points);
        }
        
        /// <summary>
        /// Obtém um ponto de coleta específico pelo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CollectionPointViewModel>> GetById(int id)
        {
            var point = await _collectionPointService.GetByIdAsync(id);
            if (point == null)
                return NotFound();
                
            return Ok(point);
        }
        
        /// <summary>
        /// Cria um novo ponto de coleta
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CollectionPointViewModel>> Create(CreateCollectionPointViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var point = await _collectionPointService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = point.Id }, point);
        }
        
        /// <summary>
        /// Atualiza um ponto de coleta existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CollectionPointViewModel>> Update(int id, UpdateCollectionPointViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var point = await _collectionPointService.UpdateAsync(id, model);
            if (point == null)
                return NotFound();
                
            return Ok(point);
        }
        
        /// <summary>
        /// Remove um ponto de coleta
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _collectionPointService.DeleteAsync(id);
            if (!result)
                return NotFound();
                
            return NoContent();
        }
        
        /// <summary>
        /// Encontra pontos de coleta próximos a uma localização
        /// </summary>
        [HttpGet("nearby")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CollectionPointViewModel>>> FindNearby(
            [FromQuery] double latitude, 
            [FromQuery] double longitude, 
            [FromQuery] double radiusKm = 5.0)
        {
            var points = await _collectionPointService.FindNearbyPointsAsync(latitude, longitude, radiusKm);
            return Ok(points);
        }
    }
}
