import { useEffect, useState } from 'react'
import { pessoasApi } from '../../services/api'
import type { Pessoa, PessoaRequest } from '../../types'

const FORM_VAZIO: PessoaRequest = { nome: '', idade: 0 }

export default function PessoasPage() {
  const [pessoas, setPessoas] = useState<Pessoa[]>([])
  const [form, setForm]       = useState<PessoaRequest>(FORM_VAZIO)
  const [editId, setEditId]   = useState<string | null>(null) // null = modo criação
  const [loading, setLoading] = useState(false)
  const [erro, setErro]       = useState('')

  useEffect(() => { carregar() }, [])

  async function carregar() {
    setPessoas(await pessoasApi.listar())
  }

  function iniciarEdicao(p: Pessoa) {
    setEditId(p.id)
    setForm({ nome: p.nome, idade: p.idade })
    setErro('')
  }

  function cancelarEdicao() {
    setEditId(null)
    setForm(FORM_VAZIO)
    setErro('')
  }

  async function salvar() {
    setErro('')
    if (!form.nome.trim()) return setErro('O nome é obrigatório.')
    if (form.idade < 0)    return setErro('A idade não pode ser negativa.')

    setLoading(true)
    try {
      // Se editId está preenchido, atualiza; caso contrário, cria uma nova pessoa.
      editId
        ? await pessoasApi.atualizar(editId, form)
        : await pessoasApi.criar(form)

      cancelarEdicao()
      await carregar()
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Erro ao salvar.')
    } finally {
      setLoading(false)
    }
  }

  async function deletar(id: string) {
    if (!confirm('Deletar esta pessoa apagará todas as suas transações. Confirma?')) return
    await pessoasApi.deletar(id)
    await carregar()
  }

  return (
    <>
      <div className="page-header">
        <h2>👤 Pessoas</h2>
      </div>

      <div className="card">
        <h3 style={{ marginBottom: 16, fontSize: 15 }}>
          {editId ? 'Editar Pessoa' : 'Nova Pessoa'}
        </h3>
        <div className="form-grid">
          <div className="field">
            <label>Nome</label>
            <input
              maxLength={200}
              value={form.nome}
              placeholder="Nome completo"
              onChange={e => setForm(f => ({ ...f, nome: e.target.value }))}
            />
          </div>
          <div className="field">
            <label>Idade</label>
            <input
              type="number"
              min={0}
              max={150}
              value={form.idade}
              onChange={e => setForm(f => ({ ...f, idade: Number(e.target.value) }))}
            />
          </div>
        </div>

        {erro && <p className="error-msg">{erro}</p>}

        <div className="form-actions">
          <button className="btn-primary" onClick={salvar} disabled={loading}>
            {loading ? 'Salvando…' : editId ? 'Salvar alterações' : 'Cadastrar'}
          </button>
          {editId && (
            <button className="btn-sm-edit" onClick={cancelarEdicao}>Cancelar</button>
          )}
        </div>
      </div>

      <div className="card">
        {pessoas.length === 0 ? (
          <p className="empty">Nenhuma pessoa cadastrada.</p>
        ) : (
          <table>
            <thead>
              <tr>
                <th>Nome</th>
                <th>Idade</th>
                <th></th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {pessoas.map(p => (
                <tr key={p.id}>
                  <td>{p.nome}</td>
                  <td>{p.idade} anos</td>
                  <td>
                    {/* Indica visualmente que a pessoa tem restrição de tipo de transação */}
                    {p.idade < 18 && (
                      <span className="badge badge-despesa">Menor de idade</span>
                    )}
                  </td>
                  <td style={{ display: 'flex', gap: 6 }}>
                    <button className="btn-sm-edit" onClick={() => iniciarEdicao(p)}>Editar</button>
                    <button className="btn-danger"   onClick={() => deletar(p.id)}>Deletar</button>
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
