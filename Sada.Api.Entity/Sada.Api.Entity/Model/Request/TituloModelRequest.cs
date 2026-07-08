using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Sada.Api.Entity.Model.Request
{
    public class TituloModelRequest
    {
        [Required]
        [StringLength(150)]
        [JsonPropertyName("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;
        
        [JsonPropertyName("vencimento")]
        public DateTime? Vencimento { get; set; }
        
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }
}
