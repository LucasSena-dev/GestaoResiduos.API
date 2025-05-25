using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoResiduos.API.Models
{
    public class Residue
    {
        public int Id { get; set; }
        public required string Name { get; set; } // Nome do resíduo
        public required string Description { get; set; }
        public required string Category { get; set; } // Ex: "Orgânico", "Plástico", etc.
        public double CurrentQuantity { get; set; } // Quantidade atual em kg
        public double AlertThreshold { get; set; } // Limite para alertas de coleta
        public bool AlertActive { get; set; } // Indica se o alerta está ativo
        public DateTime CreatedAt { get; set; }
        public DateTime? LastCollectionDate { get; set; } // Última data de coleta
    }
}