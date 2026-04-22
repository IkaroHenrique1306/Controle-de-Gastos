import { useEffect, useState } from 'react'
import { pessoasApi, categoriasApi, transacoesApi } from '../../services/api'
import type { Pessoa, Categoria, Transacao, TransacaoRequest, TipoTransacao } from '../../types'

const FORM_VAZIO: TransacaoRequest = {
  descricao: '', valor: 0, tipo: 'Despesa', categoriaId: '', pessoaId: '',
}

const fmt = (v: number) => v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })

export default function TransacoesPage() {
  const [transacoes, setTransacoes] = useState<Transacao[]>([])
  const [pessoas, setPessoas]       = useState<Pessoa[]>([])
  const [categorias, setCategorias] = useState<Categoria[]>([])
  const [form, setForm]             = useState<TransacaoRequest>(FORM_VAZIO)
  const [loading, setLoading]       = useState(false)
  const [erro, setErro]             = useState('')

  useEffect(() => {
    // Carrega pessoas, categorias e transações em paralelo para evitar waterfalls de requisição.
    Promise.all([pessoasApi.listar(), categoriasApi.listar(), transacoesApi.listar()])
      .then(([ps, cs, ts]) => { setPessoas(ps); setCategorias(cs); setTransacoes(ts) })
  }, [])

  const pessoaSelecionada = pessoas.find(p => p.id === form.pessoaId)
  const menorDeIdade      = !!pessoaSelecionada && pessoaSelecionada.idade < 18

  // Filtra categorias cujo tipo bate com o tipo de transação selecionado.
  const categoriasFiltradas = categorias.filter(c => c.finalidade === form.tipo)

  function selecionarPessoa(pessoaId: string) {
    // Se a pessoa for menor de 18, força o tipo para Despesa e limpa a categoria.
    const menor = (pessoas.find(p => p.id === pessoaId)?.idade ?? 18) < 18
    setForm(f => ({ ...f, pessoaId, ...(menor && { tipo: 'Despesa', categoriaId: '' }) }))
  }

  function selecionarTipo(tipo: TipoTransacao) {
    // Limpa a categoria ao mudar o tipo, pois a lista filtrada muda.
    setForm(f => ({ ...f, tipo, categoriaId: '' }))
  }

  async function salvar() {
    setErro('')
    if (!form.descricao.trim()) return setErro('A descrição é obrigatória.')
    if (form.valor <= 0)        return setErro('O valor deve ser maior que zero.')
    if (!form.pessoaId)         return setErro('Selecione uma pessoa.')
    if (!form.categoriaId)      return setErro('Selecione uma categoria.')

    setLoading(true)
    try {
      await transacoesApi.criar(form)
      setForm(FORM_VAZIO)
      setTransacoes(await transacoesApi.listar())
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Erro ao salvar.')
    } finally {
      setLoading(false)
    }
  }

  async function deletar(id: string) {
    if (!confirm('Excluir esta transação? Esta ação não pode ser desfeita.')) return
    await transacoesApi.deletar(id)
    setTransacoes(await transacoesApi.listar())
  }

  return (
    <>
      <div className="page-header">
        <h2>💳 Transações</h2>
      </div>

      <div className="card">
        <h3 style={{ marginBottom: 16, fontSize: 15 }}>Nova Transação</h3>

        <div className="form-grid cols-3">
          <div className="field">
            <label>Pessoa</label>
            <select value={form.pessoaId} onChange={e => selecionarPessoa(e.target.value)}>
              <option value="">Selecione…</option>
              {pessoas.map(p => (
                <option key={p.id} value={p.id}>
                  {p.nome} ({p.idade} anos{p.idade < 18 ? ' — menor' : ''})
                </option>
              ))}
            </select>
          </div>

          <div className="field">
            <label>Tipo</label>
            <select
              value={form.tipo}
              disabled={menorDeIdade} // menores só podem registrar despesas
              onChange={e => selecionarTipo(e.target.value as TipoTransacao)}
            >
              <option value="Despesa">Despesa</option>
              <option value="Receita" disabled={menorDeIdade}>Receita</option>
            </select>
            {menorDeIdade && (
              <span style={{ fontSize: 12, color: '#d97706' }}>
                ⚠️ Menor de idade — apenas despesas
              </span>
            )}
          </div>

          <div className="field">
            <label>Categoria</label>
            <select
              value={form.categoriaId}
              onChange={e => setForm(f => ({ ...f, categoriaId: e.target.value }))}
            >
              <option value="">Selecione…</option>
              {categoriasFiltradas.map(c => (
                <option key={c.id} value={c.id}>{c.descricao}</option>
              ))}
            </select>
          </div>
        </div>

        <div className="form-grid" style={{ marginTop: 16 }}>
          <div className="field">
            <label>Descrição</label>
            <input
              maxLength={400}
              value={form.descricao}
              placeholder="Ex.: Conta de luz, Salário março…"
              onChange={e => setForm(f => ({ ...f, descricao: e.target.value }))}
            />
          </div>
          <div className="field">
            <label>Valor (R$)</label>
            <input
              type="number"
              min={0.01}
              step={0.01}
              value={form.valor}
              onChange={e => setForm(f => ({ ...f, valor: Number(e.target.value) }))}
            />
          </div>
        </div>

        {erro && <p className="error-msg">{erro}</p>}

        <div className="form-actions">
          <button className="btn-primary" onClick={salvar} disabled={loading}>
            {loading ? 'Salvando…' : 'Registrar'}
          </button>
        </div>
      </div>

      <div className="card">
        {transacoes.length === 0 ? (
          <p className="empty">Nenhuma transação registrada.</p>
        ) : (
          <table>
            <thead>
              <tr>
                <th>Descrição</th>
                <th>Tipo</th>
                <th>Valor</th>
                <th>Categoria</th>
                <th>Pessoa</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {transacoes.map(t => (
                <tr key={t.id}>
                  <td>{t.descricao}</td>
                  <td>
                    <span className={`badge badge-${t.tipo.toLowerCase()}`}>{t.tipo}</span>
                  </td>
                  <td style={{ fontWeight: 600 }}>{fmt(t.valor)}</td>
                  <td>{t.categoria.descricao}</td>
                  <td>{t.pessoa.nome}</td>
                  <td>
                    <button className="btn-danger" onClick={() => deletar(t.id)}>Excluir</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </>
  )
}
