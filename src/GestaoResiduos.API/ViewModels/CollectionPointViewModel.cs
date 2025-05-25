using System;
using System.ComponentModel.DataAnnotations;

namespace GestaoResiduos.API.ViewModels
{
    public class CollectionPointViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ResponsiblePerson { get; set; } = null!;
        public string Contact { get; set; } = null!;
        public bool IsActive { get; set; }
        public string AcceptedCategories { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        
        // Propriedade calculada para mostrar categorias como lista
        public string[] AcceptedCategoriesList => 
            string.IsNullOrEmpty(AcceptedCategories) 
                ? Array.Empty<string>() 
                : AcceptedCategories.Split(',', StringSplitOptions.RemoveEmptyEntries);
    }

    public class CreateCollectionPointViewModel
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Name { get; set; } = null!;
        
        [Required(ErrorMessage = "Localização é obrigatória")]
        public string Location { get; set; } = null!;
        
        [Required(ErrorMessage = "Latitude é obrigatória")]
        [Range(-90, 90, ErrorMessage = "Latitude deve estar entre -90 e 90")]
        public double Latitude { get; set; }
        
        [Required(ErrorMessage = "Longitude é obrigatória")]
        [Range(-180, 180, ErrorMessage = "Longitude deve estar entre -180 e 180")]
        public double Longitude { get; set; }
        
        [Required(ErrorMessage = "Responsável é obrigatório")]
        public string ResponsiblePerson { get; set; } = null!;
        
        [Required(ErrorMessage = "Contato é obrigatório")]
        public string Contact { get; set; } = null!;
        
        public string AcceptedCategories { get; set; } = "";
    }

    public class UpdateCollectionPointViewModel
    {
        public string? Name { get; set; }
        public string? Location { get; set; }
        
        [Range(-90, 90, ErrorMessage = "Latitude deve estar entre -90 e 90")]
        public double? Latitude { get; set; }
        
        [Range(-180, 180, ErrorMessage = "Longitude deve estar entre -180 e 180")]
        public double? Longitude { get; set; }
        
        public string? ResponsiblePerson { get; set; }
        public string? Contact { get; set; }
        public bool? IsActive { get; set; }
        public string? AcceptedCategories { get; set; }
    }
}
