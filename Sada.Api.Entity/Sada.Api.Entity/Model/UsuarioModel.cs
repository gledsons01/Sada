namespace Sada.Api.Entity.Model;

public class UsuarioModel
{
    public int IdUsuario { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string NumeroEndereco { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public int IdUf { get; set; }
    public int IdCidade { get; set; }
    public string NomeSocial { get; set; } = string.Empty;
    public int IdSexo { get; set; }
    public string EMail { get; set; } = string.Empty;
}
