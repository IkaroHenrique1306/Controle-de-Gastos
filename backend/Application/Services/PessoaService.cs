using GastosResidenciais.API.Domain.Entities;
using GastosResidenciais.API.DTOs;
using GastosResidenciais.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.API.Application.Services;

public interface IPessoaService
{
    Task<IEnumerable<PessoaResponse>> ListarAsync();
    Task<PessoaResponse?> ObterPorIdAsync(Guid id);
    Task<PessoaResponse> CriarAsync(PessoaRequest dto);
    Task<PessoaResponse?> AtualizarAsync(Guid id, PessoaRequest dto);
    Task<bool> DeletarAsync(Guid id);
}

public class PessoaService(AppDbContext db) : IPessoaService
{
    public async Task<IEnumerable<PessoaResponse>> ListarAsync() =>
        await db.Pessoas
            .AsNoTracking()
            .Select(p => new PessoaResponse(p.Id, p.Nome, p.Idade))
            .ToListAsync();

    public async Task<PessoaResponse?> ObterPorIdAsync(Guid id)
    {
        var pessoa = await db.Pessoas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return pessoa is null ? null : new PessoaResponse(pessoa.Id, pessoa.Nome, pessoa.Idade);
    }

    public async Task<PessoaResponse> CriarAsync(PessoaRequest dto)
    {
        var pessoa = new Pessoa { Nome = dto.Nome, Idade = dto.Idade };
        db.Pessoas.Add(pessoa);
        await db.SaveChangesAsync();
        return new PessoaResponse(pessoa.Id, pessoa.Nome, pessoa.Idade);
    }

    public async Task<PessoaResponse?> AtualizarAsync(Guid id, PessoaRequest dto)
    {
        var pessoa = await db.Pessoas.FindAsync(id);
        if (pessoa is null) return null;

        pessoa.Nome = dto.Nome;
        pessoa.Idade = dto.Idade;
        await db.SaveChangesAsync();
        return new PessoaResponse(pessoa.Id, pessoa.Nome, pessoa.Idade);
    }

    // Ao remover a pessoa, o EF Core exclui automaticamente suas transações (cascade delete).
    public async Task<bool> DeletarAsync(Guid id)
    {
        var pessoa = await db.Pessoas.FindAsync(id);
        if (pessoa is null) return false;

        db.Pessoas.Remove(pessoa);
        await db.SaveChangesAsync();
        return true;
    }
}
