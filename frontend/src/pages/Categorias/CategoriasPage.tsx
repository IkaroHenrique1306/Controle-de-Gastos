import { useEffect, useState } from 'react'
import { categoriasApi } from '../../services/api'
import type { Categoria, CategoriaRequest, Finalidade } from '../../types'

const FORM_VAZIO: CategoriaRequest = { descricao: '', finalidade: 'Despesa' }

export default function CategoriasPage() {
  const [categorias, setCategorias] = useState<Categoria[]>([])
  const [form, setForm]             = useState<CategoriaRequest>(FORM_VAZIO)
  const [loading, setLoading]       = useState(false)
  const [erro, setErro]             = useState('')

  useEffect(() => { carregar() }, [])

  async function carregar() {
    setCategorias(await categoriasApi.listar())
  }

  async function salvar() {
    setErro('')
    if (!form.descricao.trim()) return setErro('A descrição é obrigatória.')

    setLoading(true)
    try {
      await categoriasApi.criar(form)
      setForm(FORM_VAZIO)
      await carregar()
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Erro ao salvar.')
    } finally {
      setLoading(false)
    }
  }

  async function deletar(id: string) {
    if (!confirm('Excluir esta categoria? Só é possível se não houver transações vinculadas.')) return
    try {
      await categoriasApi.deletar(id)
      await carregar()
    } catch (e) {
      setErro(e instanceof Error ? e.message : 'Erro ao excluir.')
    }
  }

  return (
    <>
      <div className="page-header">
        <h2>🏷️ Categorias</h2>
      </div>

      <div className="card">
        <h3 style={{ marginBottom: 16, fontSize: 15 }}>Nova Categoria</h3>
        <div className="form-grid">
          <div className="field">
            <label>Descrição</label>
            <input
              maxLength={400}
              value={form.descricao}
              placeholder="Ex.: Alimentação, Salário…"
              onChange={e => setForm(f => ({ ...f, descricao: e.target.value }))}
            />
          </div>
          <div className="field">
            <label>Finalidade</label>
            <select
              value={form.finalidade}
              onChange={e => setForm(f => ({ ...f, finalidade: e.target.value as Finalidade }))}
            >
              <option value="Despesa">Despesa</option>
              <option value="Receita">Receita</option>
            </select>
          </div>
        </div>

        {erro && <p className="error-msg">{erro}</p>}

        <div className="form-actions">
          <button className="btn-primary" onClick={salvar} disabled={loading}>
            {loading ? 'Salvando…' : 'Cadastrar'}
          </button>
        </div>
      </div>

      <div className="card">
        {categorias.length === 0 ? (
          <p className="empty">Nenhuma categoria cadastrada.</p>
        ) : (
          <table>
            <thead>
              <tr>
                <th>Descrição</th>
                <th>Finalidade</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {categorias.map(c => (
                <tr key={c.id}>
                  <td>{c.descricao}</td>
                  <td>
                    <span className={`badge badge-${c.finalidade.toLowerCase()}`}>
                      {c.finalidade}
                    </span>
                  </td>
                  <td>
                    <button className="btn-danger" onClick={() => deletar(c.id)}>Excluir</button>
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
