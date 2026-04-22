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

public class TransacaoService(AppDbContext db) : ITransacaoService
{
    // Carrega as entidades com Include e projeta em memória.
    // Necessário porque EF Core não consegue traduzir chamadas de método local para SQL.
    public async Task<IEnumerable<TransacaoResponse>> ListarAsync()
    {
        var transacoes = await db.Transacoes
            .AsNoTracking()
            .Include(t => t.Pessoa)
            .Include(t => t.Categoria)
            .ToListAsync();

        return transacoes.Select(ToResponse);
    }

    // Valida as regras de negócio antes de persistir a transação:
    // 1. Pessoa e categoria devem existir.
    // 2. Menores de 18 anos só podem registrar despesas.
    // 3. A finalidade da categoria deve ser compatível com o tipo da transação.
    public async Task<TransacaoResult> CriarAsync(TransacaoRequest dto)
    {
        var pessoa = await db.Pessoas.FindAsync(dto.PessoaId);
        if (pessoa is null)
            return new TransacaoResult(null, "Pessoa não encontrada.");

        var categoria = await db.Categorias.FindAsync(dto.CategoriaId);
        if (categoria is null)
            return new TransacaoResult(null, "Categoria não encontrada.");

        if (pessoa.Idade < 18 && dto.Tipo == TipoTransacao.Receita)
            return new TransacaoResult(null, "Pessoas menores de 18 anos só podem registrar despesas.");

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

    // A finalidade da categoria deve ser igual ao tipo da transação.
    private static bool CategoriaCompativel(FinalidadeCategoria finalidade, TipoTransacao tipo) =>
        (finalidade == FinalidadeCategoria.Despesa && tipo == TipoTransacao.Despesa) ||
        (finalidade == FinalidadeCategoria.Receita && tipo == TipoTransacao.Receita);

    private static TransacaoResponse ToResponse(Transacao t) =>
        ToResponse(t, t.Categoria, t.Pessoa);

    private static TransacaoResponse ToResponse(Transacao t, Categoria c, Pessoa p) =>
        new(t.Id, t.Descricao, t.Valor, t.Tipo,
            new CategoriaResponse(c.Id, c.Descricao, c.Finalidade),
            new PessoaResponse(p.Id, p.Nome, p.Idade));
}
