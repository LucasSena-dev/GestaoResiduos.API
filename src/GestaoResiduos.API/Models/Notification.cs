using System;

namespace GestaoResiduos.API.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string NotificationType { get; set; } = null!; // CollectionAlert, Educational, etc.
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public int? ResidueId { get; set; }
        public Residue? Residue { get; set; }
        public int? CollectionPointId { get; set; }
        public CollectionPoint? CollectionPoint { get; set; }
    }
}
