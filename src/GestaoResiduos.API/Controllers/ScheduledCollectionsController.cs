using System;
using System.Threading.Tasks;
using GestaoResiduos.API.Services;
using GestaoResiduos.API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestaoResiduos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduledCollectionsController : ControllerBase
    {
        private readonly IScheduledCollectionService _scheduledCollectionService;
        
        public ScheduledCollectionsController(IScheduledCollectionService scheduledCollectionService)
        {
            _scheduledCollectionService = scheduledCollectionService;
        }
        
        /// <summary>
        /// Obtém todas as coletas agendadas com paginação
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponse<ScheduledCollectionViewModel>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var collections = await _scheduledCollectionService.GetAllAsync(page, pageSize);
            return Ok(collections);
        }
        
        /// <summary>
        /// Obtém uma coleta agendada específica pelo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ScheduledCollectionViewModel>> GetById(int id)
        {
            var collection = await _scheduledCollectionService.GetByIdAsync(id);
            if (collection == null)
                return NotFound();
                
            return Ok(collection);
        }
        
        /// <summary>
        /// Cria uma nova coleta agendada
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ScheduledCollectionViewModel>> Create(CreateScheduledCollectionViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                    
                var collection = await _scheduledCollectionService.CreateAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = collection.Id }, collection);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Atualiza uma coleta agendada existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ScheduledCollectionViewModel>> Update(int id, UpdateScheduledCollectionViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                    
                var collection = await _scheduledCollectionService.UpdateAsync(id, model);
                if (collection == null)
                    return NotFound();
                    
                return Ok(collection);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Remove uma coleta agendada
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _scheduledCollectionService.DeleteAsync(id);
                if (!result)
                    return NotFound();
                    
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        /// <summary>
        /// Marca uma coleta como concluída
        /// </summary>
        [HttpPut("{id}/complete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ScheduledCollectionViewModel>> CompleteCollection(int id, CompleteCollectionViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                    
                var collection = await _scheduledCollectionService.CompleteCollectionAsync(id, model);
                if (collection == null)
                    return NotFound();
                    
                return Ok(collection);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
