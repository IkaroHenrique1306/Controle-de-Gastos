using System.ComponentModel.DataAnnotations;

namespace GastosResidenciais.API.Domain.Entities;

// Entidade central do domínio. Toda transação pertence a uma pessoa.
// A propriedade Idade é crítica: menores de 18 anos só podem registrar despesas — regra validada no TransacaoService.
public class Pessoa
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [Range(0, 150)]
    public int Idade { get; set; }

    // Navegação usada pelo EF Core para configurar o cascade delete no AppDbContext.
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}
