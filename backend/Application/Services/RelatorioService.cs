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

public class RelatorioService(AppDbContext db) : IRelatorioService
{
    // Carrega todas as pessoas com suas transações em uma única query (evita N+1),
    // calcula os totais individuais e depois soma o total geral.
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

    // Mesma lógica do relatório por pessoa, agrupando pelo lado da categoria.
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

    // Saldo = receitas - despesas. Valor negativo indica que a pessoa gastou mais do que recebeu.
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
