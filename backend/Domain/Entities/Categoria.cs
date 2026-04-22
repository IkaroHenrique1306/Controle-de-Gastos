using System.ComponentModel.DataAnnotations;
using GastosResidenciais.API.Domain.Enums;

namespace GastosResidenciais.API.Domain.Entities;

// Categoriza as transações financeiras.
// A Finalidade determina com quais tipos de transação esta categoria pode ser usada,
// impedindo, por exemplo, que uma categoria de "Salário" seja associada a uma despesa.
public class Categoria
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(400)]
    public string Descricao { get; set; } = string.Empty;

    public FinalidadeCategoria Finalidade { get; set; }

    // Navegação usada pelo EF Core para impedir deleção de categoria com transações vinculadas (DeleteBehavior.Restrict).
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}
