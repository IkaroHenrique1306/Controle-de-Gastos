using GastosResidenciais.API.Domain.Entities;
using GastosResidenciais.API.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.API.Infrastructure.Data;

// Contexto principal do EF Core. Usa SQLite como banco de dados —
// o arquivo gastos.db é criado automaticamente na pasta do projeto.
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Ao deletar uma pessoa, todas as suas transações são removidas automaticamente.
        builder.Entity<Pessoa>()
            .HasMany(p => p.Transacoes)
            .WithOne(t => t.Pessoa)
            .HasForeignKey(t => t.PessoaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Impede a exclusão de uma categoria que ainda possui transações vinculadas.
        builder.Entity<Categoria>()
            .HasMany(c => c.Transacoes)
            .WithOne(t => t.Categoria)
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Persiste os enums como texto no banco para facilitar leitura direta dos dados.
        builder.Entity<Categoria>()
            .Property(c => c.Finalidade)
            .HasConversion<string>();

        builder.Entity<Transacao>()
            .Property(t => t.Tipo)
            .HasConversion<string>();
    }
}
