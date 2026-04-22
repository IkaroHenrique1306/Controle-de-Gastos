using System.ComponentModel.DataAnnotations;
using GastosResidenciais.API.Domain.Enums;

namespace GastosResidenciais.API.Domain.Entities;

// Classifica as transações. Uma categoria de finalidade Despesa
// não pode ser usada em uma transação do tipo Receita, e vice-versa.
public class Categoria
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(400)]
    public string Descricao { get; set; } = string.Empty;

    public FinalidadeCategoria Finalidade { get; set; }

    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}
