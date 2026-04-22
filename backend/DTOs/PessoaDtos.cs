using System.ComponentModel.DataAnnotations;

namespace GastosResidenciais.API.DTOs;

public record PessoaRequest(
    [Required, MaxLength(200)] string Nome,
    [Range(0, 150)] int Idade
);

public record PessoaResponse(Guid Id, string Nome, int Idade);
