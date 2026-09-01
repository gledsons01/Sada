namespace Sada.Web.Models
{
    public class Titulo
    {
        public int Id_Titulo { get; set; }
        public string NomeTitulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataVencimento { get; set; } = DateTime.MinValue;
        public char Status { get; set; }
    }
}
