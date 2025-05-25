using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace GestaoResiduos.API.Models
{
    public class CollectionPoint
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome do ponto de coleta é obrigatório")]
        public string Name { get; set; } = null!; // Nome do ponto de coleta
        public string Location { get; set; } = null!;  // Ex: endereço, descrição
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ResponsiblePerson { get; set; } = null!;
        public string Contact { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public string AcceptedCategories { get; set; } = ""; // Categorias aceitas, separadas por vírgula
        public DateTime CreatedAt { get; set; }
    }
}