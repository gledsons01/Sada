using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Sada.Api.Entity.Model.Request
{
    public class CidadeModelRequest
    {
        [JsonPropertyName("idcidade")]
        public int IdCidade { get; set; }

        [StringLength(150)]
        [JsonPropertyName("descricaocidade")]
        public string? DescricaoCidade { get; set; }

        [JsonPropertyName("iduf")]
        public int IdUf { get; set; }
    }
}
