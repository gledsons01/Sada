using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sada.Api.Entity.Model.Request
{
    public class UsuarioModelRequest
    {
        public int? IdUsuario { get; set; }

        [Required]
        [StringLength(200)]
        [JsonPropertyName("nome_usuario")]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [JsonPropertyName("login")]
        public string Login { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        [JsonPropertyName("senha")]
        public string Senha { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        [JsonPropertyName("endereco")]
        public string Endereco { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [JsonPropertyName("numero_endereco")]
        public string NumeroEndereco { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [JsonPropertyName("bairro")]
        public string Bairro { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("id_uf")]
        public int? IdUf { get; set; }

        [Required]
        [JsonPropertyName("id_cidade")]
        public int? IdCidade { get; set; }

        [Required]
        [StringLength(500)]
        [JsonPropertyName("nome_social")]
        public string NomeSocial { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("id_sexo")]
        public int IdSexo { get; set; }

        [Required]
        [StringLength(500)]
        [JsonPropertyName("email")]
        public string EMail { get; set; } = string.Empty;
    }
}
