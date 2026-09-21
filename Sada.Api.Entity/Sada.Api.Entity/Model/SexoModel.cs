using System;
using System.Collections.Generic;
using System.Text;

namespace Sada.Api.Entity.Model
{
    public class SexoModel
    {
        public int IdSexo { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Sigla { get; set; } = string.Empty;
    }
}

