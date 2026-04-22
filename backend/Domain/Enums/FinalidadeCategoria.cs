namespace GastosResidenciais.API.Domain.Enums;

// Restringe quais tipos de transação podem usar uma categoria.
// Uma categoria com finalidade Despesa não pode ser associada a uma transação de Receita, e vice-versa.
public enum FinalidadeCategoria { Despesa, Receita }
