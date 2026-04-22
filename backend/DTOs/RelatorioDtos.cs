using GastosResidenciais.API.Domain.Enums;

namespace GastosResidenciais.API.DTOs;

// Saldo = TotalReceitas - TotalDespesas. Negativo indica que a pessoa/categoria gastou mais do que recebeu.
public record TotalPorPessoaResponse(
    Guid Id, string Nome,
    decimal TotalReceitas, decimal TotalDespesas, decimal Saldo
);

public record TotalPorCategoriaResponse(
    Guid Id, string Descricao, FinalidadeCategoria Finalidade,
    decimal TotalReceitas, decimal TotalDespesas, decimal Saldo
);

// Agrega os totais individuais e o consolidado geral de todas as pessoas.
public record RelatorioPorPessoaResponse(
    IEnumerable<TotalPorPessoaResponse> Pessoas,
    decimal TotalGeralReceitas, decimal TotalGeralDespesas, decimal SaldoLiquido
);

// Agrega os totais individuais e o consolidado geral de todas as categorias.
public record RelatorioPorCategoriaResponse(
    IEnumerable<TotalPorCategoriaResponse> Categorias,
    decimal TotalGeralReceitas, decimal TotalGeralDespesas, decimal SaldoLiquido
);
