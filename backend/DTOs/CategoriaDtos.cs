using System.ComponentModel.DataAnnotations;
using GastosResidenciais.API.Domain.Enums;

namespace GastosResidenciais.API.DTOs;

// Payload de entrada para criação de categoria.
// A Finalidade é obrigatória porque define quais transações podem usar esta categoria.
public record CategoriaRequest(
    [Required, MaxLength(400)] string Descricao,
    FinalidadeCategoria Finalidade
);

public record CategoriaResponse(Guid Id, string Descricao, FinalidadeCategoria Finalidade);
