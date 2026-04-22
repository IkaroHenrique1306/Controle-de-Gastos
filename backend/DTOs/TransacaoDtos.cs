using System.ComponentModel.DataAnnotations;
using GastosResidenciais.API.Domain.Enums;

namespace GastosResidenciais.API.DTOs;

public record TransacaoRequest(
    [Required, MaxLength(400)] string Descricao,
    [Range(0.01, double.MaxValue)] decimal Valor,
    TipoTransacao Tipo,
    Guid CategoriaId,
    Guid PessoaId
);

public record TransacaoResponse(
    Guid Id,
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    CategoriaResponse Categoria,
    PessoaResponse Pessoa
);

// Encapsula o resultado da criação de uma transação.
// Permite retornar dados ou um erro de regra de negócio sem lançar exceções.
public record TransacaoResult(TransacaoResponse? Data, string? Erro);
