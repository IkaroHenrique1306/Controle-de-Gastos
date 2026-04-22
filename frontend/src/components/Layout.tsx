import { NavLink, Outlet } from 'react-router-dom'

export default function Layout() {
  return (
    <div className="layout">
      <aside className="sidebar">
        <h1>💰 Gastos Residenciais</h1>
        <nav>
          <NavLink to="/pessoas">👤 Pessoas</NavLink>
          <NavLink to="/categorias">🏷️ Categorias</NavLink>
          <NavLink to="/transacoes">💳 Transações</NavLink>
          <NavLink to="/relatorios/por-pessoa">📊 Rel. por Pessoa</NavLink>
          <NavLink to="/relatorios/por-categoria">📂 Rel. por Categoria</NavLink>
        </nav>
      </aside>
      <main className="main">
        <Outlet />
      </main>
    </div>
  )
}
