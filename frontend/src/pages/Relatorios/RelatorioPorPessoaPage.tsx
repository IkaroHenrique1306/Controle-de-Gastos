import { useEffect, useState } from 'react'
import { relatoriosApi } from '../../services/api'
import type { RelatorioPorPessoa } from '../../types'

const fmt = (v: number) => v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })

export default function RelatorioPorPessoaPage() {
  const [relatorio, setRelatorio] = useState<RelatorioPorPessoa | null>(null)

  useEffect(() => { relatoriosApi.porPessoa().then(setRelatorio) }, [])

  if (!relatorio) return <p className="empty">Carregando…</p>

  return (
    <>
      <div className="page-header">
        <h2>📊 Totais por Pessoa</h2>
        <button className="btn-primary" onClick={() => relatoriosApi.porPessoa().then(setRelatorio)}>
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
        {relatorio.pessoas.length === 0 ? (
          <p className="empty">Nenhuma pessoa cadastrada.</p>
        ) : (
          <table>
            <thead>
              <tr>
                <th>Pessoa</th>
                <th>Receitas</th>
                <th>Despesas</th>
                <th>Saldo</th>
              </tr>
            </thead>
            <tbody>
              {relatorio.pessoas.map(p => (
                <tr key={p.id}>
                  <td>{p.nome}</td>
                  <td style={{ color: 'var(--success)', fontWeight: 600 }}>{fmt(p.totalReceitas)}</td>
                  <td style={{ color: 'var(--danger)',  fontWeight: 600 }}>{fmt(p.totalDespesas)}</td>
                  <td style={{ fontWeight: 700, color: p.saldo >= 0 ? 'var(--primary)' : 'var(--danger)' }}>
                    {fmt(p.saldo)}
                  </td>
                </tr>
              ))}
              <tr className="total-row">
                <td>TOTAL GERAL</td>
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
