namespace GastosResidenciais.API.Domain.Enums;

// Classifica o sentido financeiro da transação: saída (Despesa) ou entrada (Receita).
// Usado tanto para registrar a transação quanto para filtrar categorias compatíveis.
public enum TipoTransacao { Despesa, Receita }
