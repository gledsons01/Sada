using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sada.Api.Entity.Model.Request
{
    public class TituloModelRequest
    {
        [Required]
        [StringLength(150)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Descricao { get; set; } = string.Empty;
        public DateTime? Vencimento { get; set; }
        public string? Status { get; set; }
    }
}
