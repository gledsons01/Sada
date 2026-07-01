using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sada.Api.Entity.Model.Request
{
    public class UsuarioModelRequest
    {
        public int? IdUsuario { get; set; }

        [Required]
        [StringLength(200)]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Login { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Senha { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Endereco { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string NumeroEndereco { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Bairro { get; set; } = string.Empty;

        [Required]
        public int? IdUf { get; set; }

        [Required]
        public int? IdCidade { get; set; }

        [Required]
        [StringLength(500)]
        public string NomeSocial { get; set; } = string.Empty;

        [Required]
        public int IdSexo { get; set; }

        [Required]
        [StringLength(500)]
        public string EMail { get; set; } = string.Empty;
    }
}
