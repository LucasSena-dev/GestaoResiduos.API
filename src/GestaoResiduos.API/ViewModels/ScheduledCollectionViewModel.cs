using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoResiduos.API.ViewModels
{
    public class ScheduledCollectionViewModel
    {
        public int Id { get; set; }
        public int ResidueId { get; set; }
        public string ResidueName { get; set; } = null!;
        public int CollectionPointId { get; set; }
        public string CollectionPointName { get; set; } = null!;
        public string CollectionPointLocation { get; set; } = null!;
        public DateTime ScheduledDate { get; set; }
        public string Status { get; set; } = null!;
        public double EstimatedQuantity { get; set; }
        public double? ActualQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateScheduledCollectionViewModel
    {
        [Required(ErrorMessage = "ID do Resíduo é obrigatório")]
        public int ResidueId { get; set; }
        
        [Required(ErrorMessage = "ID do Ponto de Coleta é obrigatório")]
        public int CollectionPointId { get; set; }
        
        [Required(ErrorMessage = "Data de agendamento é obrigatória")]
        public DateTime ScheduledDate { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "A quantidade estimada deve ser um valor positivo")]
        public double EstimatedQuantity { get; set; }
        
        public string? Notes { get; set; }
    }

    public class UpdateScheduledCollectionViewModel
    {
        public DateTime? ScheduledDate { get; set; }
        
        public string? Status { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "A quantidade estimada deve ser um valor positivo")]
        public double? EstimatedQuantity { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "A quantidade real deve ser um valor positivo")]
        public double? ActualQuantity { get; set; }
        
        public string? Notes { get; set; }
    }

    public class CompleteCollectionViewModel
    {
        [Required(ErrorMessage = "Quantidade real coletada é obrigatória")]
        [Range(0, double.MaxValue, ErrorMessage = "A quantidade real deve ser um valor positivo")]
        public double ActualQuantity { get; set; }
        
        public string? Notes { get; set; }
    }
}
