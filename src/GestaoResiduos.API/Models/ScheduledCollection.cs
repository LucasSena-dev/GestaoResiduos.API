using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestaoResiduos.API.Models;

namespace GestaoResiduos.API.Models
{
    public class ScheduledCollection
    {
        public int Id { get; set; }
        public int ResidueId { get; set; }
        public Residue Residue { get; set; } = null!;
        
        public int CollectionPointId { get; set; }
        public CollectionPoint CollectionPoint { get; set; } = null!;
        
        public DateTime ScheduledDate { get; set; }
        public string Status { get; set; } = null!; // Ex: Pending, Completed, Cancelled
        public double EstimatedQuantity { get; set; } // Quantidade estimada em kg
        public double? ActualQuantity { get; set; } // Quantidade real coletada em kg
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Notes { get; set; }
    }
}