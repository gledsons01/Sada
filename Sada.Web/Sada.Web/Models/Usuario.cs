namespace Sada.Web.Models
{
    public class Usuario
    {
        public int Id_Usuario { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public int Id_Uf { get; set; }
        public int Id_Cidade { get; set; }
        public string NomeSocial { get; set; } = string.Empty;
        public int Id_Sexo { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
