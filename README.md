# Chamados

Sistema de chamados (ordens de serviço) para manutenção predial, desenvolvido ao longo do curso **Desenvolvimento Web e Multiplataforma com .NET**.

Estado atual: **Módulo 3**. O domínio e a API JSON do Módulo 2 ganharam uma interface web com Razor Pages: painel, lista com filtros, formulário de abertura e página de detalhes onde o chamado muda de status.

## Requisitos

- .NET SDK 10 (versão fixada em `global.json`, com `rollForward: latestFeature`)
- Visual Studio 2026 ou VS Code com C# Dev Kit
- Git

## Como executar

```
dotnet build
dotnet run --project src/Chamados.Web
```

Use o perfil `https`: o Módulo 3 liga o redirecionamento para HTTPS. Em Development, a aplicação popula a base em memória com chamados de exemplo.

## Como testar

```
dotnet test
```

Resultado esperado: **28 testes aprovados** (17 em `ChamadoTests` e 11 em `ChamadoServicoTests`).

## Estrutura

```
chamados/
├── global.json
├── Chamados.slnx
├── src/
│   ├── Chamados.Domain/      regras de negócio, sem dependência de web
│   └── Chamados.Web/         Razor Pages, API JSON e composição (Program.cs)
└── tests/
    └── Chamados.Domain.Tests/
```

## Rotas

### Páginas (Razor Pages)

| Rota | Descrição |
|---|---|
| `/` | Painel com estatísticas |
| `/Chamados` | Lista com filtros por status, prioridade mínima e texto |
| `/Chamados/Novo` | Formulário de abertura de chamado |
| `/Chamados/Detalhes/{id}` | Detalhes e histórico, com ações de iniciar atendimento, resolver e cancelar |
| `/Erro` | Página de erro (usada fora de Development) |

### API JSON (`/api`)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/chamados` | Lista, com filtros `status`, `prioridadeMinima` e `texto` |
| GET | `/api/chamados/estatisticas` | Totais por status e por prioridade, tempo médio de resolução |
| GET | `/api/chamados/{id}` | Consulta um chamado |
| POST | `/api/chamados` | Abre um chamado |
| POST | `/api/chamados/{id}/atendimento` | Inicia o atendimento |
| GET | `/api/status` | Lista os status possíveis |
| GET | `/api/ambiente` | Diagnóstico, somente em Development |

Respostas de erro da API: 404 (chamado inexistente), 409 (transição de status inválida) e 400 (dados inválidos).

## Decisões

1. **.NET 10 (LTS) como alvo.** Suporte até 14/11/2028, o que cobre todo o curso. O .NET 8 e o .NET 9 perdem suporte em 10/11/2026.
2. **Formato `.slnx`.** É o padrão do SDK 10 para arquivos de solução: XML curto, sem GUIDs.
3. **Interface de repositório (`IChamadoRepositorio`).** Existe porque a implementação em memória será substituída por Entity Framework Core no Módulo 7.
4. **`TimeProvider` injetado.** O relógio é uma dependência do serviço, o que permite testar datas e durações com um relógio controlado (`RelogioFixo`).
5. **Casos de uso no Domain, sem camada Application por enquanto.** Uma camada extra só acrescentaria referências sem benefício real nesta fase.
6. **API sob `/api` com um filtro único de exceções.** As rotas JSON ficam num grupo (`MapGroup`) para não disputar caminhos com as páginas, já que o roteamento não diferencia maiúsculas (`/Chamados` e `/chamados` seriam a mesma rota). Um filtro de endpoint converte as exceções de domínio em 404, 409 e 400, no lugar dos `try/catch` repetidos do Módulo 2.
7. **A página conversa com o serviço, nunca com o repositório.** Páginas e API usam o mesmo `ChamadoServico`, então nenhuma regra de negócio mora no projeto Web.
8. **A tela valida para ser gentil, o domínio valida para garantir.** Os limites de tamanho (título e descrição) são constantes do domínio, reutilizadas nos atributos de validação do formulário, de modo que existe uma única fonte de verdade. A validação da tela dá mensagens amigáveis, e o domínio continua rejeitando dados inválidos mesmo que a tela seja contornada.
9. **Post/Redirect/Get.** Depois de um POST bem-sucedido, a página redireciona e a mensagem de sucesso viaja por `TempData`. Assim, atualizar a página (F5) não reenvia o formulário nem repete a transição.

Outras escolhas do módulo: configuração tipada (`SimulacaoOptions`) validada na inicialização com `ValidateOnStart`; tradução de enums para texto de tela (`Rotulos`) mantida no projeto Web, por ser decisão de apresentação; `UseExceptionHandler` e `UseHsts` ativos apenas fora de Development.