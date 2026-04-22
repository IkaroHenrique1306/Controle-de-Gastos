using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GastosResidenciais.API.Domain.Enums;

namespace GastosResidenciais.API.Domain.Entities;

// Representa uma movimentação financeira vinculada a uma pessoa e a uma categoria.
public class Transacao
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(400)]
    public string Descricao { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Valor { get; set; }

    public TipoTransacao Tipo { get; set; }

    public Guid CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public Guid PessoaId { get; set; }
    public Pessoa Pessoa { get; set; } = null!;
}
