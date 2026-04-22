using GastosResidenciais.API.Domain.Entities;
using GastosResidenciais.API.DTOs;
using GastosResidenciais.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.API.Application.Services;

public interface ICategoriaService
{
    Task<IEnumerable<CategoriaResponse>> ListarAsync();
    Task<CategoriaResponse?> ObterPorIdAsync(Guid id);
    Task<CategoriaResponse> CriarAsync(CategoriaRequest dto);
    Task<(bool Removida, string? Erro)> DeletarAsync(Guid id);
}

public class CategoriaService(AppDbContext db) : ICategoriaService
{
    public async Task<IEnumerable<CategoriaResponse>> ListarAsync() =>
        await db.Categorias
            .AsNoTracking()
            .Select(c => new CategoriaResponse(c.Id, c.Descricao, c.Finalidade))
            .ToListAsync();

    public async Task<CategoriaResponse?> ObterPorIdAsync(Guid id)
    {
        var categoria = await db.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return categoria is null ? null : new CategoriaResponse(categoria.Id, categoria.Descricao, categoria.Finalidade);
    }

    public async Task<CategoriaResponse> CriarAsync(CategoriaRequest dto)
    {
        var categoria = new Categoria { Descricao = dto.Descricao, Finalidade = dto.Finalidade };
        db.Categorias.Add(categoria);
        await db.SaveChangesAsync();
        return new CategoriaResponse(categoria.Id, categoria.Descricao, categoria.Finalidade);
    }

    // Impede a exclusão se houver transações vinculadas, pois o banco usa DeleteBehavior.Restrict.
    // Retorna uma mensagem de erro descritiva para ser exibida ao usuário.
    public async Task<(bool Removida, string? Erro)> DeletarAsync(Guid id)
    {
        var categoria = await db.Categorias
            .Include(c => c.Transacoes)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (categoria is null)
            return (false, null);

        if (categoria.Transacoes.Any())
            return (false, "Não é possível excluir uma categoria que possui transações vinculadas.");

        db.Categorias.Remove(categoria);
        await db.SaveChangesAsync();
        return (true, null);
    }
}
