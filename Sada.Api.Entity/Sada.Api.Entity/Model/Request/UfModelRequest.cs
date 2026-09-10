using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sada.Api.Entity.Model.Request
{
    public class UfModelRequest
    {
        [JsonPropertyName("iduf")]
        public int IdUf { get; set; }

        [StringLength(2)]
        [JsonPropertyName("siglauf")]
        public string? SiglaUf { get; set; }

        [StringLength(2)]
        [JsonPropertyName("sigla")]
        public string? Sigla { get; set; }
    }
}
