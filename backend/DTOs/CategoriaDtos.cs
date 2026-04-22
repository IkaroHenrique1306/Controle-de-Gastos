using System.ComponentModel.DataAnnotations;
using GastosResidenciais.API.Domain.Enums;

namespace GastosResidenciais.API.DTOs;

public record CategoriaRequest(
    [Required, MaxLength(400)] string Descricao,
    FinalidadeCategoria Finalidade
);

public record CategoriaResponse(Guid Id, string Descricao, FinalidadeCategoria Finalidade);
