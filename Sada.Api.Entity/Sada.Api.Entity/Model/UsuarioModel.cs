namespace Sada.Api.Entity.Model;

public class UsuarioModel
{
    public int IdUsuario { get; set; }
    public string? NomeUsuario { get; set; }
    public string? Login { get; set; }
    public string? Senha { get; set; }
    public string? Endereco { get; set; }
    public string? NumeroEndereco { get; set; }
    public string? Bairro { get; set; }
    public int? IdUf { get; set; }
    public int? IdCidade { get; set; }
    public string? NomeSocial { get; set; }
    public int? ID_SEXO { get; set; }
    public string? E_MAIL { get; set; }
}
