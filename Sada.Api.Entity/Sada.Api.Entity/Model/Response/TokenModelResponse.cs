using System;
using System.Collections.Generic;
using System.Text;

namespace Sada.Api.Entity.Model.Response
{
    public class TokenModelResponse
    {
        public string? Token { get; set; }
        public DateTime? TokenExpiraEm { get; set; }
        public string? RefreshToken { get; set; }
        public int? IdUsuarioToken { get; set; }
        public DateTime? RefreshTokenExpiraEm { get; set; }
    }
}
