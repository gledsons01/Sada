using System;
using System.Collections.Generic;
using System.Text;

namespace Sada.Api.Entity.Model.Response
{
    public class LoginModelResponse
    {
        public int? IdUsuario { get; set; }
        public string? Token { get; set; }
        public string? RrefreshToken { get; set; }
    }
}
