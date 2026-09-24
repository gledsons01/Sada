using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sada.Api.Entity.Model.Response
{
    public class SexoModelResponse
    {
        [JsonPropertyName("idsexo")]
        public int IdSexo { get; set; }

        [JsonPropertyName("descricao")]
        public string? Descricao { get; set; }
        
        [JsonPropertyName("sigla")]
        public string? Sigla { get; set; }
    }
}
