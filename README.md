# Gastos Residenciais

Sistema web para controle de gastos residenciais. Permite cadastrar pessoas, categorias e transações financeiras, com relatórios de receitas, despesas e saldo por pessoa e por categoria.

---

## Tecnologias

| Camada       | Tecnologia                                          |
|--------------|-----------------------------------------------------|
| Back-end     | C# 12 · .NET 8 · ASP.NET Core Web API              |
| Persistência | SQLite · Entity Framework Core 8                    |
| Front-end    | React 18 · TypeScript · Vite · React Router v6     |

---

## Arquitetura

```
backend/
├── Controllers/          # Endpoints HTTP — sem lógica de negócio
├── Application/
│   └── Services/         # Regras de negócio
├── Domain/
│   ├── Entities/         # Entidades do domínio (Pessoa, Categoria, Transacao)
│   └── Enums/            # Enumerações (TipoTransacao, FinalidadeCategoria)
├── Infrastructure/
│   └── Data/             # AppDbContext (EF Core + SQLite)
└── DTOs/                 # Contratos de entrada e saída da API (por domínio)
    ├── PessoaDtos.cs
    ├── CategoriaDtos.cs
    ├── TransacaoDtos.cs
    └── RelatorioDtos.cs

frontend/
└── src/
    ├── pages/       # Uma pasta por funcionalidade
    ├── components/  # Componentes compartilhados
    ├── services/    # Camada de acesso à API (api.ts)
    └── types/       # Interfaces TypeScript espelhando os DTOs
```

**Decisões técnicas:**

- **SQLite** — elimina dependência de servidor externo. O arquivo `gastos.db` é criado automaticamente na pasta `backend/` na primeira execução.
- **`record` para DTOs** — imutabilidade por padrão, sintaxe concisa, igualdade estrutural.
- **Primary constructors** (C# 12) nos services — reduz boilerplate de injeção de dependência.
- **`TransacaoResult`** — pattern de resultado explícito para erros de negócio, sem lançar exceções para fluxo esperado.
- **Enums serializados como string** — JSON legível (`"Despesa"` em vez de `0`) e banco de dados interpretável sem lookup.
- **`EnsureCreated`** — cria o schema automaticamente sem necessidade de migrations manuais, adequado para projetos de demonstração.
- **`AsNoTracking`** em todas as queries de leitura — performance melhorada em operações somente-leitura.
- **Origem CORS em `appsettings.json`** — facilita trocar a URL do front-end sem recompilar.

---

## Regras de negócio

| Regra | Onde é aplicada |
|-------|-----------------|
| Menores de 18 anos só podem registrar **despesas** | Back-end (Service) + Front-end (UI) |
| A finalidade da categoria deve ser compatível com o tipo da transação | Back-end (Service) + Front-end (filtra o select) |
| Deletar uma pessoa remove todas as suas transações | EF Core — `DeleteBehavior.Cascade` |
| Deletar uma categoria é bloqueado se houver transações vinculadas | EF Core — `DeleteBehavior.Restrict` + validação no Service |
| Valor da transação deve ser positivo | `DataAnnotations` + validação no front-end |

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)

---

## Como executar

### Windows

Clique duas vezes em `start.bat`.  
Instala dependências automaticamente (se necessário), sobe os dois serviços em uma única janela e abre o navegador em `http://localhost:5173`.

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

| Serviço   | URL                              |
|-----------|----------------------------------|
| Front-end | http://localhost:5173            |
| API       | http://localhost:5000            |
| Swagger   | http://localhost:5000/swagger    |

---

## Endpoints da API

### Pessoas
| Método   | Rota                  | Descrição                              |
|----------|-----------------------|----------------------------------------|
| `GET`    | `/api/pessoas`        | Lista todas                            |
| `GET`    | `/api/pessoas/{id}`   | Busca por id                           |
| `POST`   | `/api/pessoas`        | Cria                                   |
| `PUT`    | `/api/pessoas/{id}`   | Atualiza                               |
| `DELETE` | `/api/pessoas/{id}`   | Remove + remove transações (cascade)   |

### Categorias
| Método   | Rota                     | Descrição                                    |
|----------|--------------------------|----------------------------------------------|
| `GET`    | `/api/categorias`        | Lista todas                                  |
| `POST`   | `/api/categorias`        | Cria                                         |
| `DELETE` | `/api/categorias/{id}`   | Remove — bloqueado se houver transações (409) |

### Transações
| Método   | Rota                     | Descrição                              |
|----------|--------------------------|----------------------------------------|
| `GET`    | `/api/transacoes`        | Lista todas                            |
| `POST`   | `/api/transacoes`        | Cria — valida regras de negócio (422)  |
| `DELETE` | `/api/transacoes/{id}`   | Remove                                 |

### Relatórios
| Método | Rota                           | Descrição                              |
|--------|--------------------------------|----------------------------------------|
| `GET`  | `/api/relatorios/por-pessoa`   | Totais por pessoa + total geral        |
| `GET`  | `/api/relatorios/por-categoria`| Totais por categoria + total geral     |
