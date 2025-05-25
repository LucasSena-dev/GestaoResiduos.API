using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoResiduos.API.ViewModels
{
    public class ResidueViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public double CurrentQuantity { get; set; }
        public double AlertThreshold { get; set; }
        public bool AlertActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastCollectionDate { get; set; }
    }

    public class CreateResidueViewModel
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Name { get; set; } = null!;
        
        [Required(ErrorMessage = "Descrição é obrigatória")]
        public string Description { get; set; } = null!;
        
        [Required(ErrorMessage = "Categoria é obrigatória")]
        public string Category { get; set; } = null!;
        
        [Range(0, double.MaxValue, ErrorMessage = "A quantidade atual deve ser um valor positivo")]
        public double CurrentQuantity { get; set; } = 0;
        
        [Range(0, double.MaxValue, ErrorMessage = "O limite de alerta deve ser um valor positivo")]
        public double AlertThreshold { get; set; } = 100;
    }

    public class UpdateResidueViewModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "A quantidade atual deve ser um valor positivo")]
        public double? CurrentQuantity { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "O limite de alerta deve ser um valor positivo")]
        public double? AlertThreshold { get; set; }
    }
}
