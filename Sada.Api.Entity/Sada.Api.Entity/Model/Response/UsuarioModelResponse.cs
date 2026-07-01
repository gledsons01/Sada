using System;
using System.Collections.Generic;
using System.Text;

namespace Sada.Api.Entity.Model.Response
{
    public class UsuarioModelResponse
    {
        public int IdUsuario { get; set; }
        public bool blnRetorno { get; set; }
        public string? NomeUsuario { get; set; }
        public string? Login { get; set; }
        public string? Senha { get; set; }
        public string? Endereco { get; set; }
        public string? NumeroEndereco { get; set; }
        public string? Bairro { get; set; } 
        public int? IdUf { get; set; }
        public int? IdCidade { get; set; }
        public string? NomeSocial { get; set; }
        public string? EMail { get; set; }
        public bool? blnRetornp { get; set; }
        public int? IdSexo { get; set; }
    }
}
