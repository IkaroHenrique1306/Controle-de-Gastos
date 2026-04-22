using System.ComponentModel.DataAnnotations;
using GastosResidenciais.API.Domain.Enums;

namespace GastosResidenciais.API.DTOs;

// Payload de entrada para criação de transação.
// Valor mínimo de 0.01 garante que não é possível registrar transação zerada ou negativa.
public record TransacaoRequest(
    [Required, MaxLength(400)] string Descricao,
    [Range(0.01, double.MaxValue)] decimal Valor,
    TipoTransacao Tipo,
    Guid CategoriaId,
    Guid PessoaId
);

// Retorna os dados completos da categoria e da pessoa para evitar lookups adicionais no cliente.
public record TransacaoResponse(
    Guid Id,
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    CategoriaResponse Categoria,
    PessoaResponse Pessoa
);

// Padrão de resultado para evitar uso de exceções em fluxos de negócio esperados.
// Se Erro for nulo, a operação foi bem-sucedida e Data contém o resultado.
public record TransacaoResult(TransacaoResponse? Data, string? Erro);
