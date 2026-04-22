using System.ComponentModel.DataAnnotations;

namespace GastosResidenciais.API.DTOs;

// DTOs isolam o contrato da API da entidade de domínio.
// Mudanças na entidade não afetam o contrato público da API enquanto os DTOs não mudarem.

// Payload de entrada para criação e atualização. As annotations validam antes de chegar ao service.
public record PessoaRequest(
    [Required, MaxLength(200)] string Nome,
    [Range(0, 150)] int Idade
);

// Payload de saída. Expõe apenas o que o cliente precisa — sem navigation properties do EF.
public record PessoaResponse(Guid Id, string Nome, int Idade);
