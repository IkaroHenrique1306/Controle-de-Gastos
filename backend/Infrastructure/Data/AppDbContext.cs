using GastosResidenciais.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GastosResidenciais.API.Infrastructure.Data;

// Ponto de acesso ao banco de dados via EF Core.
// Usa SQLite: sem necessidade de servidor externo. O arquivo gastos.db é criado automaticamente pelo EnsureCreated no Program.cs.
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Cascade delete: remover uma Pessoa apaga todas as suas Transações automaticamente.
        // Evita registros órfãos sem precisar de lógica extra no serviço.
        builder.Entity<Pessoa>()
            .HasMany(p => p.Transacoes)
            .WithOne(t => t.Pessoa)
            .HasForeignKey(t => t.PessoaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict: impede deletar uma Categoria que ainda tem Transações vinculadas.
        // O CategoriaService valida isso antes de tentar remover, retornando uma mensagem amigável.
        builder.Entity<Categoria>()
            .HasMany(c => c.Transacoes)
            .WithOne(t => t.Categoria)
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Enums persistidos como string para o banco ser legível sem precisar de lookup de valores numéricos.
        builder.Entity<Categoria>()
            .Property(c => c.Finalidade)
            .HasConversion<string>();

        builder.Entity<Transacao>()
            .Property(t => t.Tipo)
            .HasConversion<string>();
    }
}
