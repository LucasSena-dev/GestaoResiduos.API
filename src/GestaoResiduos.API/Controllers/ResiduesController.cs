using System.Threading.Tasks;
using GestaoResiduos.API.Services;
using GestaoResiduos.API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestaoResiduos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResiduesController : ControllerBase
    {
        private readonly IResidueService _residueService;
        
        public ResiduesController(IResidueService residueService)
        {
            _residueService = residueService;
        }
        
        /// <summary>
        /// Obtém todos os resíduos com paginação
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponse<ResidueViewModel>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var residues = await _residueService.GetAllAsync(page, pageSize);
            return Ok(residues);
        }
        
        /// <summary>
        /// Obtém um resíduo específico pelo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResidueViewModel>> GetById(int id)
        {
            var residue = await _residueService.GetByIdAsync(id);
            if (residue == null)
                return NotFound();
                
            return Ok(residue);
        }
        
        /// <summary>
        /// Cria um novo resíduo
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResidueViewModel>> Create(CreateResidueViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var residue = await _residueService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = residue.Id }, residue);
        }
        
        /// <summary>
        /// Atualiza um resíduo existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResidueViewModel>> Update(int id, UpdateResidueViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var residue = await _residueService.UpdateAsync(id, model);
            if (residue == null)
                return NotFound();
                
            return Ok(residue);
        }
        
        /// <summary>
        /// Remove um resíduo
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _residueService.DeleteAsync(id);
            if (!result)
                return NotFound();
                
            return NoContent();
        }
        
        /// <summary>
        /// Verifica e atualiza os status de alerta para todos os resíduos
        /// </summary>
        [HttpPost("check-alerts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckAlerts()
        {
            var result = await _residueService.CheckAndUpdateAlertStatusesAsync();
            return Ok(new { AlertsUpdated = result });
        }
    }
}
