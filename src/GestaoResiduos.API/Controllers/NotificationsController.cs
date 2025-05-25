using System.Threading.Tasks;
using GestaoResiduos.API.Services;
using GestaoResiduos.API.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestaoResiduos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        
        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        
        /// <summary>
        /// Obtém todas as notificações com paginação
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponse<NotificationViewModel>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var notifications = await _notificationService.GetAllAsync(page, pageSize);
            return Ok(notifications);
        }
        
        /// <summary>
        /// Obtém uma notificação específica pelo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotificationViewModel>> GetById(int id)
        {
            var notification = await _notificationService.GetByIdAsync(id);
            if (notification == null)
                return NotFound();
                
            return Ok(notification);
        }
        
        /// <summary>
        /// Cria uma nova notificação
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NotificationViewModel>> Create(CreateNotificationViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var notification = await _notificationService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = notification.Id }, notification);
        }
        
        /// <summary>
        /// Marca uma notificação como lida
        /// </summary>
        [HttpPut("{id}/read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _notificationService.MarkAsReadAsync(id);
            if (!result)
                return NotFound();
                
            return Ok(new { Success = true });
        }
        
        /// <summary>
        /// Atualiza uma notificação existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NotificationViewModel>> Update(int id, UpdateNotificationViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var notification = await _notificationService.UpdateAsync(id, model);
            if (notification == null)
                return NotFound();
                
            return Ok(notification);
        }
        
        /// <summary>
        /// Remove uma notificação
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _notificationService.DeleteAsync(id);
            if (!result)
                return NotFound();
                
            return NoContent();
        }
        
        /// <summary>
        /// Obtém a contagem de notificações não lidas
        /// </summary>
        [HttpGet("unread-count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetUnreadCount()
        {
            var count = await _notificationService.GetUnreadCountAsync();
            return Ok(new { UnreadCount = count });
        }
    }
}
