using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Sada.Api.Entity.Model.Request
{
    public class LoginModelRequest
    {
        [Required]
        [StringLength(20)]
        [JsonPropertyName("login")]
        public string Login { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        [JsonPropertyName("senha")]
        public string Senha { get; set; } = string.Empty;
    }
}
