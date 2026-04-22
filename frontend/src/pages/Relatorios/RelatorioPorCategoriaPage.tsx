import { useEffect, useState } from 'react'
import { relatoriosApi } from '../../services/api'
import type { RelatorioPorCategoria } from '../../types'

const fmt = (v: number) => v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })

export default function RelatorioPorCategoriaPage() {
  const [relatorio, setRelatorio] = useState<RelatorioPorCategoria | null>(null)

  useEffect(() => { relatoriosApi.porCategoria().then(setRelatorio) }, [])

  if (!relatorio) return <p className="empty">Carregando…</p>

  return (
    <>
      <div className="page-header">
        <h2>📂 Totais por Categoria</h2>
        <button className="btn-primary" onClick={() => relatoriosApi.porCategoria().then(setRelatorio)}>
          Atualizar
        </button>
      </div>

      <div className="summary-row">
        <div className="summary-card">
          <div className="label">Total Receitas</div>
          <div className="value value-receita">{fmt(relatorio.totalGeralReceitas)}</div>
        </div>
        <div className="summary-card">
          <div className="label">Total Despesas</div>
          <div className="value value-despesa">{fmt(relatorio.totalGeralDespesas)}</div>
        </div>
        <div className="summary-card">
          <div className="label">Saldo Líquido</div>
          <div className={`value ${relatorio.saldoLiquido >= 0 ? 'value-saldo-pos' : 'value-saldo-neg'}`}>
            {fmt(relatorio.saldoLiquido)}
          </div>
        </div>
      </div>

      <div className="card">
        {relatorio.categorias.length === 0 ? (
          <p className="empty">Nenhuma categoria cadastrada.</p>
        ) : (
          <table>
            <thead>
              <tr>
                <th>Categoria</th>
                <th>Finalidade</th>
                <th>Receitas</th>
                <th>Despesas</th>
                <th>Saldo</th>
              </tr>
            </thead>
            <tbody>
              {relatorio.categorias.map(c => (
                <tr key={c.id}>
                  <td>{c.descricao}</td>
                  <td>
                    <span className={`badge badge-${c.finalidade.toLowerCase()}`}>
                      {c.finalidade}
                    </span>
                  </td>
                  <td style={{ color: 'var(--success)', fontWeight: 600 }}>{fmt(c.totalReceitas)}</td>
                  <td style={{ color: 'var(--danger)',  fontWeight: 600 }}>{fmt(c.totalDespesas)}</td>
                  <td style={{ fontWeight: 700, color: c.saldo >= 0 ? 'var(--primary)' : 'var(--danger)' }}>
                    {fmt(c.saldo)}
                  </td>
                </tr>
              ))}
              <tr className="total-row">
                <td colSpan={2}>TOTAL GERAL</td>
                <td>{fmt(relatorio.totalGeralReceitas)}</td>
                <td>{fmt(relatorio.totalGeralDespesas)}</td>
                <td>{fmt(relatorio.saldoLiquido)}</td>
              </tr>
            </tbody>
          </table>
        )}
      </div>
    </>
  )
}
