# 💰 Sistema de Controle de Gastos Residenciais

Sistema web fullstack para controle de gastos residenciais, com gerenciamento de pessoas, categorias e transações financeiras, incluindo relatórios consolidados por pessoa e categoria.

---

## Tecnologias e onde cada uma foi usada

### C# 12
- Definição das entidades de domínio (`Pessoa`, `Categoria`, `Transacao`) com Data Annotations para validação
- Implementação dos services com toda a lógica de negócio (`TransacaoService`, `PessoaService`, etc.)
- **Primary constructors** nos services para injeção de dependência sem boilerplate
- **Records** para os DTOs — imutabilidade por padrão e sintaxe concisa
- **Pattern de resultado** (`TransacaoResult`) para comunicar erros de negócio sem lançar exceções

### .NET 8 / ASP.NET Core
- Hospedagem e execução da Web API
- **Roteamento** via atributos `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`
- **`[ApiController]`** para validação automática de `ModelState` e binding por convenção
- **`[ProducesResponseType]`** em todos os endpoints para documentação precisa no Swagger
- **Injeção de dependência** via `AddScoped` — cada request tem sua própria instância do service
- **Serialização de enums como string** via `JsonStringEnumConverter` (`"Despesa"` em vez de `0`)
- **CORS** configurado via `appsettings.json` para não exigir recompilação ao mudar de ambiente
- **Swagger/OpenAPI** para documentação e testes dos endpoints em desenvolvimento

### Entity Framework Core 8
- Mapeamento objeto-relacional das entidades para o banco SQLite
- **`DeleteBehavior.Cascade`** — deletar uma `Pessoa` remove automaticamente todas as suas `Transacoes`
- **`DeleteBehavior.Restrict`** — impede deletar uma `Categoria` com transações vinculadas
- **`HasConversion<string>()`** — enums persistidos como texto no banco para legibilidade direta
- **`AsNoTracking()`** em todas as queries de leitura para evitar overhead de rastreamento desnecessário
- **`EnsureCreated`** — cria o schema automaticamente na primeira execução, sem migrations manuais
- **`Include()`** nas queries de relatório para carregar as navegações em uma única query e evitar N+1

### SQLite
- Banco de dados relacional embutido, sem necessidade de servidor externo
- Arquivo `gastos.db` criado automaticamente na pasta `backend/` na primeira execução
- Tipos numéricos monetários mapeados como `decimal(18,2)` para precisão sem arredondamentos

### React 18
- Todas as telas da aplicação (`PessoasPage`, `CategoriasPage`, `TransacoesPage`, `RelatorioPorPessoaPage`, `RelatorioPorCategoriaPage`)
- **`useState`** para gerenciar estado local de formulários, listas e mensagens de erro
- **`useEffect`** para disparar o carregamento de dados ao montar cada página
- **`Promise.all`** em `TransacoesPage` para carregar pessoas, categorias e transações em paralelo, evitando waterfall de requisições
- **React Router v6** para navegação entre páginas sem recarregar o browser
- **`NavLink`** no `Layout` para aplicar a classe `active` automaticamente na rota atual

### TypeScript
- Tipagem estática de todas as interfaces que espelham os DTOs do back-end (`Pessoa`, `Categoria`, `Transacao`, etc.)
- Tipos de `Finalidade` e `TipoTransacao` como union types (`'Despesa' | 'Receita'`) para garantir contratos corretos com a API
- Tipagem dos payloads de entrada (`PessoaRequest`, `TransacaoRequest`) e saída (`RelatorioPorPessoa`, etc.)
- Wrapper genérico `request<T>` em `api.ts` com inferência de tipo para todas as chamadas HTTP

### Vite
- Bundler e servidor de desenvolvimento do front-end
- Flag `--open` no `start.bat` e `start.sh` para abrir o navegador automaticamente quando o servidor estiver pronto

---

## Arquitetura do back-end

```
backend/
├── Controllers/          # Endpoints HTTP — sem lógica de negócio, apenas delega ao service
├── Application/
│   └── Services/         # Toda a lógica de negócio e regras de domínio
├── Domain/
│   ├── Entities/         # Entidades mapeadas pelo EF Core (Pessoa, Categoria, Transacao)
│   └── Enums/            # TipoTransacao · FinalidadeCategoria
├── Infrastructure/
│   └── Data/             # AppDbContext — configuração do EF Core e relacionamentos
└── DTOs/                 # Contratos de entrada e saída da API, separados por domínio
    ├── PessoaDtos.cs
    ├── CategoriaDtos.cs
    ├── TransacaoDtos.cs
    └── RelatorioDtos.cs
```

---

## Regras de negócio

| Regra | Onde é aplicada |
|-------|-----------------|
| Menores de 18 anos só podem registrar **despesas** | `TransacaoService` (back-end) + select desabilitado (front-end) |
| Finalidade da categoria deve ser compatível com o tipo da transação | `TransacaoService` (back-end) + filtro no select (front-end) |
| Deletar uma pessoa remove todas as suas transações | `DeleteBehavior.Cascade` no `AppDbContext` |
| Deletar uma categoria é bloqueado se houver transações vinculadas | `DeleteBehavior.Restrict` + validação no `CategoriaService` |
| Valor da transação deve ser positivo | `[Range(0.01, double.MaxValue)]` + validação no front-end |

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)

---

## Como executar

### Windows

Clique duas vezes em `start.bat` — instala dependências (se necessário), sobe os dois serviços em uma única janela e abre o navegador automaticamente em `http://localhost:5173`.

### Linux / Mac

```bash
chmod +x start.sh
./start.sh
```

### Manual

```bash
# Back-end — porta 5000
cd backend
dotnet run

# Front-end — porta 5173
cd frontend
npm install
npm run dev
```

| Serviço   | URL                           |
|-----------|-------------------------------|
| Front-end | http://localhost:5173         |
| API       | http://localhost:5000         |
| Swagger   | http://localhost:5000/swagger |

---

## Endpoints da API

### Pessoas
| Método   | Rota                | Descrição                            |
|----------|---------------------|--------------------------------------|
| `GET`    | `/api/pessoas`      | Lista todas                          |
| `GET`    | `/api/pessoas/{id}` | Busca por id                         |
| `POST`   | `/api/pessoas`      | Cria                                 |
| `PUT`    | `/api/pessoas/{id}` | Atualiza                             |
| `DELETE` | `/api/pessoas/{id}` | Remove + remove transações (cascade) |

### Categorias
| Método   | Rota                   | Descrição                                     |
|----------|------------------------|-----------------------------------------------|
| `GET`    | `/api/categorias`      | Lista todas                                   |
| `POST`   | `/api/categorias`      | Cria                                          |
| `DELETE` | `/api/categorias/{id}` | Remove — bloqueado se houver transações (409) |

### Transações
| Método   | Rota                   | Descrição                             |
|----------|------------------------|---------------------------------------|
| `GET`    | `/api/transacoes`      | Lista todas                           |
| `POST`   | `/api/transacoes`      | Cria — valida regras de negócio (422) |
| `DELETE` | `/api/transacoes/{id}` | Remove                                |

### Relatórios
| Método | Rota                            | Descrição                         |
|--------|---------------------------------|-----------------------------------|
| `GET`  | `/api/relatorios/por-pessoa`    | Totais por pessoa + total geral   |
| `GET`  | `/api/relatorios/por-categoria` | Totais por categoria + total geral|

## Demo

![GIF preview](assets/demo.gif)

📺 Vídeo completo: [https://youtube.com/...](https://youtu.be/rihD8gAND88)
