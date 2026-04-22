import { NavLink, Outlet } from 'react-router-dom'

// Estrutura visual fixa da aplicação: sidebar de navegação + área de conteúdo.
// <Outlet /> é onde o React Router injeta o componente da rota ativa.
// NavLink aplica a classe "active" automaticamente na rota atual — usada pelo CSS para destacar o item.
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
