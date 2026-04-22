using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GastosResidenciais.API.Domain.Enums;

namespace GastosResidenciais.API.Domain.Entities;

// Representa uma movimentação financeira — despesa ou receita — vinculada a uma Pessoa e a uma Categoria.
// Apenas transações com Categoria de finalidade compatível com o Tipo podem ser criadas.
public class Transacao
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(400)]
    public string Descricao { get; set; } = string.Empty;

    // decimal(18,2) garante precisão monetária sem arredondamentos indesejados.
    [Column(TypeName = "decimal(18,2)")]
    public decimal Valor { get; set; }

    public TipoTransacao Tipo { get; set; }

    public Guid CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public Guid PessoaId { get; set; }
    public Pessoa Pessoa { get; set; } = null!;
}
