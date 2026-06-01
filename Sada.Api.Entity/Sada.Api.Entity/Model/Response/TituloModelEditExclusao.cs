using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sada.Api.Entity.Model.Response
{
    public class TituloModelEditExclusao
    {
        public int IdTitulo { get; set; }
                
        public DateTime? Vencimento { get; set; }
        
        [Required]
        public string? Status { get; set; }
    }
}
