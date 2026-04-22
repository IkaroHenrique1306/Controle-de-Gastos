// Tipos que espelham os DTOs e enums do back-end.
// Mantê-los em sincronia garante que os dados trafegados com a API sejam sempre do formato esperado.

export type Finalidade = 'Despesa' | 'Receita'
export type TipoTransacao = 'Despesa' | 'Receita'

export interface Pessoa {
  id: string
  nome: string
  idade: number
}

export interface Categoria {
  id: string
  descricao: string
  finalidade: Finalidade
}

export interface Transacao {
  id: string
  descricao: string
  valor: number
  tipo: TipoTransacao
  categoria: Categoria
  pessoa: Pessoa
}

// Payloads enviados à API
export interface PessoaRequest { nome: string; idade: number }
export interface CategoriaRequest { descricao: string; finalidade: Finalidade }
export interface TransacaoRequest {
  descricao: string
  valor: number
  tipo: TipoTransacao
  categoriaId: string
  pessoaId: string
}

// Estrutura retornada pelos endpoints de relatório
export interface TotalPorPessoa {
  id: string; nome: string
  totalReceitas: number; totalDespesas: number; saldo: number
}

export interface TotalPorCategoria {
  id: string; descricao: string; finalidade: Finalidade
  totalReceitas: number; totalDespesas: number; saldo: number
}

export interface RelatorioPorPessoa {
  pessoas: TotalPorPessoa[]
  totalGeralReceitas: number; totalGeralDespesas: number; saldoLiquido: number
}

export interface RelatorioPorCategoria {
  categorias: TotalPorCategoria[]
  totalGeralReceitas: number; totalGeralDespesas: number; saldoLiquido: number
}
