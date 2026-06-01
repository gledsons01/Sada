namespace Sada.Api.Entity.Model;

public class TituloModel
{
    public int IdTitulo { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime? Vencimento { get; set; }
    public char? Status { get; set; }
}
