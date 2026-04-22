using GastosResidenciais.API.Domain.Entities;
using GastosResidenciais.API.Domain.Enums;
using GastosResidenciais.API.DTOs;
using GastosResidenciais.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.API.Application.Services;

public interface IRelatorioService
{
    Task<RelatorioPorPessoaResponse> TotaisPorPessoaAsync();
    Task<RelatorioPorCategoriaResponse> TotaisPorCategoriaAsync();
}

// Consolida dados financeiros para exibição em relatórios.
// Não persiste dados — apenas lê e agrega.
public class RelatorioService(AppDbContext db) : IRelatorioService
{
    // Uma única query com Include traz pessoas e transações juntas, evitando o problema N+1.
    // O cálculo de totais é feito em memória após o carregamento.
    public async Task<RelatorioPorPessoaResponse> TotaisPorPessoaAsync()
    {
        var pessoas = await db.Pessoas
            .AsNoTracking()
            .Include(p => p.Transacoes)
            .ToListAsync();

        var totais = pessoas.Select(CalcularTotalPorPessoa).ToList();

        return new RelatorioPorPessoaResponse(
            totais,
            totais.Sum(t => t.TotalReceitas),
            totais.Sum(t => t.TotalDespesas),
            totais.Sum(t => t.Saldo)
        );
    }

    // Mesma estratégia do relatório por pessoa, agora agrupando pelo lado da categoria.
    public async Task<RelatorioPorCategoriaResponse> TotaisPorCategoriaAsync()
    {
        var categorias = await db.Categorias
            .AsNoTracking()
            .Include(c => c.Transacoes)
            .ToListAsync();

        var totais = categorias.Select(CalcularTotalPorCategoria).ToList();

        return new RelatorioPorCategoriaResponse(
            totais,
            totais.Sum(t => t.TotalReceitas),
            totais.Sum(t => t.TotalDespesas),
            totais.Sum(t => t.Saldo)
        );
    }

    // Saldo negativo indica que a pessoa gastou mais do que recebeu no período.
    private static TotalPorPessoaResponse CalcularTotalPorPessoa(Pessoa p)
    {
        var receitas = p.Transacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor);
        var despesas = p.Transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor);
        return new TotalPorPessoaResponse(p.Id, p.Nome, receitas, despesas, receitas - despesas);
    }

    private static TotalPorCategoriaResponse CalcularTotalPorCategoria(Categoria c)
    {
        var receitas = c.Transacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor);
        var despesas = c.Transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor);
        return new TotalPorCategoriaResponse(c.Id, c.Descricao, c.Finalidade, receitas, despesas, receitas - despesas);
    }
}
