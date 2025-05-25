using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoResiduos.API.ViewModels
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string NotificationType { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ResidueId { get; set; }
        public string? ResidueName { get; set; }
        public int? CollectionPointId { get; set; }
        public string? CollectionPointName { get; set; }
    }

    public class CreateNotificationViewModel
    {
        [Required(ErrorMessage = "Título é obrigatório")]
        public string Title { get; set; } = null!;
        
        [Required(ErrorMessage = "Mensagem é obrigatória")]
        public string Message { get; set; } = null!;
        
        [Required(ErrorMessage = "Tipo de notificação é obrigatório")]
        public string NotificationType { get; set; } = null!;
        
        public int? ResidueId { get; set; }
        
        public int? CollectionPointId { get; set; }
    }

    public class UpdateNotificationViewModel
    {
        public bool? IsRead { get; set; }
    }
}
