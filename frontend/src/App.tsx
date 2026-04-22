import { Navigate, Route, Routes } from 'react-router-dom'
import Layout from './components/Layout'
import PessoasPage from './pages/Pessoas/PessoasPage'
import CategoriasPage from './pages/Categorias/CategoriasPage'
import TransacoesPage from './pages/Transacoes/TransacoesPage'
import RelatorioPorPessoaPage from './pages/Relatorios/RelatorioPorPessoaPage'
import RelatorioPorCategoriaPage from './pages/Relatorios/RelatorioPorCategoriaPage'

// Raiz de roteamento da aplicação.
// Todas as rotas vivem dentro de Layout, que fornece a sidebar e a área de conteúdo.
// A rota "/" redireciona para "/pessoas" como página inicial padrão.
export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<Navigate to="/pessoas" replace />} />
        <Route path="pessoas"    element={<PessoasPage />} />
        <Route path="categorias" element={<CategoriasPage />} />
        <Route path="transacoes" element={<TransacoesPage />} />
        <Route path="relatorios/por-pessoa"    element={<RelatorioPorPessoaPage />} />
        <Route path="relatorios/por-categoria" element={<RelatorioPorCategoriaPage />} />
      </Route>
    </Routes>
  )
}
