using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sada.Api.Entity.Model.Response
{
    public class TituloModelResponse
    {
        public int IdTitulo { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime? Vencimento { get; set; }
        public char? Status { get; set; }
        public bool Retorno { get; set; }
    }
}
