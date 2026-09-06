using System;
using System.Collections.Generic;
using System.Text;

namespace Sada.Api.Entity.Model.Response
{
    public class CidadeModelResponse
    {
        public int IdCidade { get; set; }
        public int IdUf { get; set; }
        public string? DescricaoCidade { get; set; }
    }
}
