import type {
  Pessoa, PessoaRequest,
  Categoria, CategoriaRequest,
  Transacao, TransacaoRequest,
  RelatorioPorPessoa, RelatorioPorCategoria,
} from '../types'

const BASE_URL = 'http://localhost:5000/api'

// Wrapper genérico para todas as chamadas HTTP.
// Em caso de erro, tenta extrair a mensagem enviada pela API antes de lançar a exceção.
async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...init,
  })

  if (!res.ok) {
    const body = await res.json().catch(() => null)
    throw new Error(body?.erro ?? `Erro ${res.status}`)
  }

  if (res.status === 204) return undefined as T
  return res.json()
}

const post = <T>(path: string, body: unknown) =>
  request<T>(path, { method: 'POST', body: JSON.stringify(body) })

const put = <T>(path: string, body: unknown) =>
  request<T>(path, { method: 'PUT', body: JSON.stringify(body) })

const del = (path: string) =>
  request<void>(path, { method: 'DELETE' })

export const pessoasApi = {
  listar:    ()                                => request<Pessoa[]>('/pessoas'),
  criar:     (data: PessoaRequest)             => post<Pessoa>('/pessoas', data),
  atualizar: (id: string, data: PessoaRequest) => put<Pessoa>(`/pessoas/${id}`, data),
  deletar:   (id: string)                      => del(`/pessoas/${id}`),
}

export const categoriasApi = {
  listar:  ()                             => request<Categoria[]>('/categorias'),
  criar:   (data: CategoriaRequest)       => post<Categoria>('/categorias', data),
  deletar: (id: string)                   => del(`/categorias/${id}`),
}

export const transacoesApi = {
  listar:  ()                             => request<Transacao[]>('/transacoes'),
  criar:   (data: TransacaoRequest)       => post<Transacao>('/transacoes', data),
  deletar: (id: string)                   => del(`/transacoes/${id}`),
}

export const relatoriosApi = {
  porPessoa:    () => request<RelatorioPorPessoa>('/relatorios/por-pessoa'),
  porCategoria: () => request<RelatorioPorCategoria>('/relatorios/por-categoria'),
}
