using GastosResidenciais.API.Application.Services;
using GastosResidenciais.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// SQLite: sem necessidade de servidor externo. Ideal para demonstração e desenvolvimento local.
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite("Data Source=gastos.db"));

// Registro via interface: facilita substituição de implementação e suporte a testes unitários.
builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();

// Enums serializado como string no JSON ("Despesa" em vez de 0) para contratos de API mais legíveis.
builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()));

// Origens permitidas no appsettings.json para facilitar mudança de ambiente sem recompilar.
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()!;
builder.Services.AddCors(o =>
    o.AddPolicy("FrontEnd", p =>
        p.WithOrigins(allowedOrigins)
         .AllowAnyHeader()
         .AllowAnyMethod()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// EnsureCreated cria o banco e as tabelas na primeira execução, sem necessidade de migrations manuais.
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontEnd");
app.UseAuthorization();
app.MapControllers();

app.Run();
