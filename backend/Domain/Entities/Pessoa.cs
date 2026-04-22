using System.ComponentModel.DataAnnotations;
using GastosResidenciais.API.Domain.Entities;

namespace GastosResidenciais.API.Domain.Entities;

// Representa uma pessoa que pode registrar transações financeiras.
// A idade é usada para restringir o tipo de transação permitida (menores de 18 só podem ter despesas).
public class Pessoa
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [Range(0, 150)]
    public int Idade { get; set; }

    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}
