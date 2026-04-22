using GastosResidenciais.API.Domain.Entities;
using GastosResidenciais.API.Domain.Enums;
using GastosResidenciais.API.DTOs;
using GastosResidenciais.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.API.Application.Services;

public interface ITransacaoService
{
    Task<IEnumerable<TransacaoResponse>> ListarAsync();
    Task<TransacaoResult> CriarAsync(TransacaoRequest dto);
    Task<bool> DeletarAsync(Guid id);
}

// Centraliza todas as regras de negócio relacionadas a transações financeiras.
public class TransacaoService(AppDbContext db) : ITransacaoService
{
    // Carrega com Include e projeta em memória após o ToListAsync.
    // EF Core não consegue traduzir chamadas de métodos locais (como ToResponse) para SQL —
    // tentar usar Select antes do ToListAsync lançaria InvalidOperationException em runtime.
    public async Task<IEnumerable<TransacaoResponse>> ListarAsync()
    {
        var transacoes = await db.Transacoes
            .AsNoTracking()
            .Include(t => t.Pessoa)
            .Include(t => t.Categoria)
            .ToListAsync();

        return transacoes.Select(ToResponse);
    }

    // Ponto central de validação de regras de negócio antes de persistir.
    // Ordem das validações: existência das entidades → restrição de idade → compatibilidade de categoria.
    // Retorna TransacaoResult em vez de lançar exceção para manter o fluxo de erro explícito e controlado.
    public async Task<TransacaoResult> CriarAsync(TransacaoRequest dto)
    {
        var pessoa = await db.Pessoas.FindAsync(dto.PessoaId);
        if (pessoa is null)
            return new TransacaoResult(null, "Pessoa não encontrada.");

        var categoria = await db.Categorias.FindAsync(dto.CategoriaId);
        if (categoria is null)
            return new TransacaoResult(null, "Categoria não encontrada.");

        // Regra de negócio: menores de 18 anos só podem ter despesas.
        if (pessoa.Idade < 18 && dto.Tipo == TipoTransacao.Receita)
            return new TransacaoResult(null, "Pessoas menores de 18 anos só podem registrar despesas.");

        // Regra de negócio: a finalidade da categoria deve corresponder ao tipo da transação.
        if (!CategoriaCompativel(categoria.Finalidade, dto.Tipo))
            return new TransacaoResult(null,
                $"A categoria '{categoria.Descricao}' não é compatível com transações do tipo '{dto.Tipo}'.");

        var transacao = new Transacao
        {
            Descricao   = dto.Descricao,
            Valor       = dto.Valor,
            Tipo        = dto.Tipo,
            CategoriaId = dto.CategoriaId,
            PessoaId    = dto.PessoaId
        };

        db.Transacoes.Add(transacao);
        await db.SaveChangesAsync();

        return new TransacaoResult(ToResponse(transacao, categoria, pessoa), null);
    }

    public async Task<bool> DeletarAsync(Guid id)
    {
        var transacao = await db.Transacoes.FindAsync(id);
        if (transacao is null) return false;

        db.Transacoes.Remove(transacao);
        await db.SaveChangesAsync();
        return true;
    }

    // A compatibilidade é 1-para-1: Despesa só aceita Despesa, Receita só aceita Receita.
    private static bool CategoriaCompativel(FinalidadeCategoria finalidade, TipoTransacao tipo) =>
        (finalidade == FinalidadeCategoria.Despesa && tipo == TipoTransacao.Despesa) ||
        (finalidade == FinalidadeCategoria.Receita && tipo == TipoTransacao.Receita);

    // Sobrecarga para projeção em memória após listagem (navigation properties já carregadas).
    private static TransacaoResponse ToResponse(Transacao t) =>
        ToResponse(t, t.Categoria, t.Pessoa);

    // Sobrecarga usada logo após criação, onde a entidade ainda não tem navigation properties populadas.
    private static TransacaoResponse ToResponse(Transacao t, Categoria c, Pessoa p) =>
        new(t.Id, t.Descricao, t.Valor, t.Tipo,
            new CategoriaResponse(c.Id, c.Descricao, c.Finalidade),
            new PessoaResponse(p.Id, p.Nome, p.Idade));
}
