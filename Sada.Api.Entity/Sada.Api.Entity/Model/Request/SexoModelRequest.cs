using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Sada.Api.Entity.Model.Request
{
    public class SexoModelRequest
    {
        [JsonPropertyName("idsexo")]
        public int IdSexo { get; set; }

        [StringLength(30)]
        [JsonPropertyName("descricao")]
        public string? Descricao { get; set; }

        [StringLength(02)]
        [JsonPropertyName("sigla")]
        public string? Sigla { get; set; }
    }
}
