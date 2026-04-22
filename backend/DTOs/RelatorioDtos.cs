using GastosResidenciais.API.Domain.Enums;

namespace GastosResidenciais.API.DTOs;

public record TotalPorPessoaResponse(
    Guid Id, string Nome,
    decimal TotalReceitas, decimal TotalDespesas, decimal Saldo
);

public record TotalPorCategoriaResponse(
    Guid Id, string Descricao, FinalidadeCategoria Finalidade,
    decimal TotalReceitas, decimal TotalDespesas, decimal Saldo
);

// Inclui os totais individuais e o consolidado geral de todas as pessoas.
public record RelatorioPorPessoaResponse(
    IEnumerable<TotalPorPessoaResponse> Pessoas,
    decimal TotalGeralReceitas, decimal TotalGeralDespesas, decimal SaldoLiquido
);

// Inclui os totais individuais e o consolidado geral de todas as categorias.
public record RelatorioPorCategoriaResponse(
    IEnumerable<TotalPorCategoriaResponse> Categorias,
    decimal TotalGeralReceitas, decimal TotalGeralDespesas, decimal SaldoLiquido
);
