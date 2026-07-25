# SolidarityConnection

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoftsqlserver)
![MongoDB](https://img.shields.io/badge/MongoDB-7.0-47A248?logo=mongodb)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3-FF6600?logo=rabbitmq)
![Kubernetes](https://img.shields.io/badge/Kubernetes-EKS%20%7C%20Local-326CE5?logo=kubernetes)
![License](https://img.shields.io/badge/license-academic--project-lightgrey)

> Repositório principal do **SolidarityConnection (Conexão Solidária)** — API + Frontend. Contém todas as regras de negócio da plataforma.

Este projeto foi desenvolvido como parte do **Tech Challenge da Pós-Graduação em Arquitetura de Software (FIAP)**, com o objetivo de aplicar, em um cenário real de hackathon, conceitos de Clean Architecture, CQRS, mensageria assíncrona, múltiplos modelos de persistência e observabilidade completa.

## Sumário

- [Sobre o projeto](#sobre-o-projeto)
- [Arquitetura](#arquitetura)
- [Stack técnica](#stack-técnica)
- [Estrutura do repositório](#estrutura-do-repositório)
- [Como subir o projeto](#como-subir-o-projeto)
- [Secrets e variáveis de ambiente](#secrets-e-variáveis-de-ambiente)
- [Endpoints da API](#endpoints-da-api)
- [Fluxo de uma doação](#fluxo-de-uma-doação)
- [Frontend e Dashboard de Transparência](#frontend-e-dashboard-de-transparência)
- [Observabilidade](#observabilidade)
- [Repositórios relacionados](#repositórios-relacionados)

---

## Sobre o projeto

O SolidarityConnection é uma plataforma de gestão de doações para ONGs. Existem dois perfis de usuário:

- **GestorONG**: cria e administra campanhas de arrecadação, gerencia usuários e acompanha o recebimento de doações.
- **Doador**: se cadastra, consulta campanhas ativas, realiza doações e acompanha seus próprios totais doados por campanha.

Toda doação passa por um fluxo assíncrono de processamento (via mensageria), e o resultado desse processamento alimenta tanto o banco transacional quanto um **dashboard público de transparência**, que mostra a arrecadação de cada campanha sem expor dados sensíveis.

## Arquitetura

O sistema é dividido em dois microsserviços (mais a infraestrutura, versionada em repositório separado):

- **SolidarityConnection** (este repositório): API + Frontend. Concentra toda a regra de negócio, autenticação, criação de campanhas/doações e o modelo de leitura da transparência.
- **SCDonationProcessor**: Worker responsável por processar a doação recebida (simular/decidir aprovação) e devolver o resultado via fila.

```
Doador ──► POST /api/donation ──► [SolidarityConnection API]
                                        │
                                        ▼ publica DonationReceivedEvent
                                 fila: donation-received
                                        │
                                        ▼
                             [SCDonationProcessor Worker]
                                        │
                                        ▼ publica DonationProcessedEvent
                                 fila: donation-processed
                                        │
                                        ▼ (consumido pela própria API)
                     Atualiza status da doação (SQL Server)
                     + Atualiza saldo da campanha (SQL Server)
                     + Registra na visão de transparência (MongoDB)
```

A API publica e também consome mensagens do RabbitMQ: publica `donation-received` quando uma doação é criada, e consome `donation-processed` (publicado pelo Worker) para atualizar o status da doação e o saldo da campanha.

Para o desenho completo da infraestrutura (AWS/EKS, VPC, RDS, etc.), consulte o diagrama de arquitetura entregue junto ao Tech Challenge e o repositório [SolidarityConnectionDeployFile](#repositórios-relacionados).

## Stack técnica

| Camada | Tecnologia |
|---|---|
| Linguagem / Runtime | C# / .NET 8 |
| API | ASP.NET Core Web API (Controllers) |
| Frontend | Blazor WebAssembly + MudBlazor (hospedado pela própria API) |
| Dashboard de transparência | HTML estático + Chart.js (servido em `/transparencia`) |
| Banco transacional (write model) | SQL Server 2022 + Entity Framework Core 8 |
| Banco de leitura (read model / CQRS) | MongoDB 7 |
| Mensageria | RabbitMQ (RabbitMQ.Client 6.8.1) |
| Autenticação | JWT (Bearer) + BCrypt para hashing de senha |
| Jobs em background | Quartz.NET |
| Observabilidade | Prometheus, Grafana, Zabbix, Jaeger (OpenTelemetry) |
| Testes | xUnit, Moq, FluentAssertions, Bogus |
| Orquestração | Kubernetes (EKS em produção, Docker Desktop em ambiente local) |
| CI/CD | GitHub Actions |

**Arquitetura de software**: Clean Architecture / Onion, com CQRS implementado manualmente (sem MediatR, por questão de licenciamento).

## Estrutura do repositório

```
SolidarityConnection/
├── SolidarityConnection.Domain/          # Entidades, Value Objects, enums e regras de domínio
├── SolidarityConnection.Application/     # Casos de uso (Commands/Queries), interfaces
├── SolidarityConnection.Infrastructure/  # EF Core, RabbitMQ, MongoDB, JWT, Quartz jobs, DI
├── SolidarityConnection.Presentation/    # Controllers, Program.cs, Dockerfile, wwwroot (dashboard)
├── SolidarityConnection.Frontend/        # Blazor WebAssembly (MudBlazor)
├── SolidarityConnection.Tests/           # Testes unitários (xUnit)
├── observability/                        # docker-compose com Prometheus/Grafana/Zabbix/Jaeger
└── .github/workflows/                    # Pipeline de CI/CD (build, testes, scan, deploy no EKS)
```

> A pasta `observability/` sobe apenas a stack de monitoramento isoladamente (útil para inspecionar dashboards). Ela **não** sobe SQL Server, MongoDB ou RabbitMQ — essa parte é feita pelo repositório de infraestrutura, descrito a seguir.

## Como subir o projeto

O ambiente local **não** é feito via `docker-compose` simples: API, Worker e toda a infraestrutura (SQL Server, MongoDB, RabbitMQ, Jaeger, Prometheus, Grafana, Zabbix) sobem juntos como um cluster Kubernetes local, através de um script no repositório [SolidarityConnectionDeployFile](https://github.com/dairussi/SolidarityConnectionDeployFile). Esse script builda as imagens Docker diretamente a partir do código-fonte deste repositório e do `SCDonationProcessor`, então **qualquer pessoa** com os três repositórios clonados consegue subir o ambiente completo do zero.

### Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) com **Kubernetes habilitado** (contexto `docker-desktop`)
- `kubectl`
- Git Bash (Windows) ou um shell bash (Linux/macOS)
- .NET 8 SDK (necessário apenas se for compilar/rodar algo fora dos containers, ex: rodar os testes)

### Passo a passo

1. **Clone os três repositórios** lado a lado, na mesma pasta raiz:
   ```bash
   git clone https://github.com/dairussi/SolidarityConnection.git
   git clone https://github.com/dairussi/SCDonationProcessor.git
   git clone https://github.com/dairussi/SolidarityConnectionDeployFile.git
   ```

2. **Configure o ambiente local**, dentro de `SolidarityConnectionDeployFile/k8s-local`:
   ```bash
   cd SolidarityConnectionDeployFile/k8s-local
   cp .env.local.example .env.local   # se o arquivo de exemplo existir; senão, use .env.local como base
   ```
   Edite o `.env.local` e ajuste:
   - `API_REPO_PATH` e `WORKER_REPO_PATH`: caminhos absolutos, no seu disco, para os repositórios `SolidarityConnection` e `SCDonationProcessor` (use `/` mesmo no Windows, o script roda em bash)
   - As demais senhas (`SA_PASSWORD`, `MONGODB_ROOT_PASSWORD`, `RABBITMQ_PASSWORD`, `JWT_SECRET_KEY`, `ADMIN_SEED_PASSWORD`, `GRAFANA_ADMIN_PASSWORD`, `ZABBIX_DB_PASSWORD`) — veja a seção [Secrets](#secrets-e-variáveis-de-ambiente) para o significado de cada uma.

3. **Garanta que o `kubectl` está apontando para o Docker Desktop**:
   ```bash
   kubectl config use-context docker-desktop
   ```

4. **Suba o ambiente**:
   ```bash
   ./subir_local.sh
   ```
   O script builda as imagens da API (runtime + migrations) e do Worker, aplica os manifests do Kubernetes, cria os Secrets/ConfigMaps, sobe SQL Server/MongoDB/RabbitMQ/Jaeger, aguarda tudo ficar saudável, roda o Job de migration do banco e por fim sobe API e Worker.

5. **Acesse os endereços expostos** (impressos ao final do script):

   | Serviço | Endereço local |
   |---|---|
   | API (Swagger em `/swagger`) | http://localhost:30080 |
   | Dashboard de transparência | http://localhost:30080/transparencia |
   | Frontend (Blazor) | http://localhost:30080 |
   | RabbitMQ Management | http://localhost:30672 |
   | Jaeger UI | http://localhost:30686 |
   | Prometheus | http://localhost:30090 |
   | Grafana | http://localhost:30300 |
   | Zabbix Web | http://localhost:30880 |

6. **Para derrubar o ambiente** e limpar tudo (namespace, imagens locais):
   ```bash
   ./limpar_local.sh
   ```

> Login inicial (usuário administrador criado pelo seed): veja a seção de [Secrets](#secrets-e-variáveis-de-ambiente).

### Rodando apenas os testes automatizados

Sem precisar subir nenhuma infraestrutura, para rodar a suíte de testes unitários:

```bash
dotnet restore SolidarityConnection.sln
dotnet test SolidarityConnection.sln
```

## Secrets e variáveis de ambiente

> ⚠️ Todos os valores abaixo estão **mascarados**. Nunca faça commit de senhas reais — no CI/CD elas ficam nos GitHub Secrets, e localmente ficam no `.env.local` (que não deve ser versionado).

### Configurações da aplicação (`appsettings.json` / variáveis de ambiente)

| Chave | Descrição |
|---|---|
| `ConnectionStrings:DefaultConnection` | String de conexão do SQL Server (banco transacional) |
| `Jwt:SecretKey` | Chave simétrica usada para assinar/validar os tokens JWT |
| `RabbitMQ:Host` / `Port` / `Username` / `Password` | Conexão com o RabbitMQ |
| `RabbitMQ:DonationReceivedQueue` / `DonationProcessedQueue` | Nomes das filas do fluxo de doação |
| `Mongo:ConnectionString` | String de conexão do MongoDB (read model de transparência) |
| `Mongo:DatabaseName` / `CollectionName` | Nome do banco/coleção usados pelo dashboard de transparência |
| `AdminSeed:Email` / `Password` / `CPF` | Dados do usuário administrador criado automaticamente no primeiro start |
| `Jaeger:OtlpEndpoint` | Endpoint OTLP para exportação de traces |
| `BackgroundJobs:PendingDonationReprocessing:IntervalInMinutes` | Intervalo do job que reprocessa doações pendentes |
| `BackgroundJobs:CampaignTransparencyBackfill:IntervalInMinutes` | Intervalo do job que reconcilia o read model de transparência |

### Secrets configurados no GitHub Actions (deploy para EKS)

| Secret | Descrição |
|---|---|
| `DOCKER_USERNAME` / `DOCKER_PASSWORD` | Credenciais do Docker Hub para push das imagens |
| `AWS_ACCESS_KEY_ID` / `AWS_SECRET_ACCESS_KEY` / `AWS_SESSION_TOKEN` | Credenciais temporárias da AWS usadas para deploy no EKS |
| `SOLIDARITY_CONNECTION_DB_CONNECTION_STRING` | Connection string do RDS (SQL Server gerenciado) |
| `JWT_SECRET_KEY` | Chave JWT usada em produção |
| `ADMIN_SEED_PASSWORD` | Senha do usuário administrador em produção |
| `RABBITMQ_HOST` / `PORT` / `USERNAME` / `PASSWORD` | Conexão com o RabbitMQ do cluster EKS |

### Credenciais de ambiente local (`.env.local`, ver [SolidarityConnectionDeployFile](https://github.com/dairussi/SolidarityConnectionDeployFile))

| Variável | Exemplo mascarado |
|---|---|
| `SA_PASSWORD` | `********` (senha do `sa` no SQL Server local) |
| `MONGODB_ROOT_USERNAME` / `MONGODB_ROOT_PASSWORD` | `********` |
| `RABBITMQ_USERNAME` / `RABBITMQ_PASSWORD` | `********` |
| `GRAFANA_ADMIN_USER` / `GRAFANA_ADMIN_PASSWORD` | `********` |
| `ZABBIX_DB_USER` / `ZABBIX_DB_PASSWORD` / `ZABBIX_DB_NAME` | `********` |

### Acesso administrativo padrão (seed)

Um usuário `GestorONG` é criado automaticamente na primeira subida da aplicação, para permitir o primeiro acesso sem precisar de cadastro manual:

- **E-mail:** `admin@solidarityconnection.com`
- **Senha:** definida via `AdminSeed:Password` / secret `ADMIN_SEED_PASSWORD` (mascarada acima — solicite o valor real com quem administra o ambiente)

## Endpoints da API

Documentação interativa completa disponível via Swagger em `/swagger` após subir a aplicação. Resumo dos grupos principais:

### Auth
| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| POST | `/api/auth/login` | Público | Autentica e retorna o token JWT |

### User
| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| POST | `/api/user/registration` | Público | Cadastra um novo usuário (Doador ou GestorONG) |
| GET | `/api/user` | GestorONG | Lista usuários paginados |
| GET | `/api/user/ById` | Autenticado | Busca usuário por Id |
| PATCH | `/api/user/Role` | GestorONG | Altera o papel (role) de um usuário |

### Campaign
| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| POST | `/api/campaign` | GestorONG | Cria uma campanha |
| GET | `/api/campaign/{id}` | Autenticado | Busca campanha por Id |
| GET | `/api/campaign` | Autenticado | Lista campanhas paginadas |
| GET | `/api/campaign/active` | Público | Lista campanhas ativas (usado pelo Doador) |
| PATCH | `/api/campaign/{id}/status` | GestorONG | Atualiza status da campanha (Active/Paused/Closed) |
| DELETE | `/api/campaign/{id}` | GestorONG | Remove uma campanha |
| GET | `/api/campaign/transparency-dashboard` | Público | Dados agregados para o dashboard de transparência (lê do MongoDB) |

### Donation
| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| POST | `/api/donation` | Autenticado (Doador) | Registra uma doação (retorna 202 Accepted; processamento é assíncrono) |
| GET | `/api/donation/MyTotalsByCampaign` | Autenticado | Retorna o total já doado pelo usuário logado, por campanha |

### Outros
| Método | Rota | Descrição |
|---|---|---|
| GET | `/health` | Health check (SQL Server + RabbitMQ) |
| GET | `/metrics` | Métricas no formato Prometheus |

## Fluxo de uma doação

1. O Doador chama `POST /api/donation`. A API valida os dados e retorna `202 Accepted` imediatamente — **a doação ainda não foi persistida como paga**, apenas registrada como `Pending`.
2. A API publica um `DonationReceivedEvent` na fila `donation-received`.
3. O `SCDonationProcessor` (Worker, repositório separado) consome essa mensagem, decide o resultado do processamento e publica um `DonationProcessedEvent` na fila `donation-processed`.
4. A própria API consome `donation-processed` (`DonationProcessedConsumer`), e:
   - Atualiza o status da doação (`Pending` → `Paid` ou `Rejected`) no SQL Server;
   - Se aprovada, soma o valor ao saldo da campanha no SQL Server;
   - Se aprovada, registra a doação no MongoDB (read model), alimentando o dashboard de transparência.
5. Um job do Quartz (`PendingDonationReprocessingJob`) reprocessa periodicamente doações que ficaram pendentes por tempo demais, e outro job (`CampaignTransparencyBackfillJob`) reconcilia o read model de transparência com o banco transacional.

O consumidor é idempotente: eventos duplicados ou fora de ordem (ex: um evento antigo chegando depois de a doação já estar paga) são identificados e ignorados, evitando inconsistência entre os dois bancos.

## Frontend e Dashboard de Transparência

- **Frontend principal**: aplicação Blazor WebAssembly (MudBlazor), hospedada pela própria API. Contém as telas de Login, Cadastro, Home, Criação de Campanha, Doações e Gestão de Usuários — os menus disponíveis variam conforme o papel do usuário logado (Doador ou GestorONG).
- **Dashboard de transparência** (`/transparencia`): página HTML estática com Chart.js, servida diretamente pela API. Consome o endpoint público `GET /api/campaign/transparency-dashboard` e não exige autenticação — qualquer pessoa pode acompanhar quanto cada campanha já arrecadou.

## Observabilidade

A aplicação expõe métricas Prometheus em `/metrics` e traces distribuídos via OpenTelemetry (exportados para o Jaeger). No ambiente local completo (via `subir_local.sh`), a stack de observabilidade sobe automaticamente:

| Ferramenta | Finalidade |
|---|---|
| Prometheus | Coleta de métricas (HTTP, runtime .NET, etc.) |
| Grafana | Dashboards sobre as métricas coletadas |
| Jaeger | Tracing distribuído das requisições e chamadas ao RabbitMQ |
| Zabbix | Monitoramento de infraestrutura/host |

Também é possível subir apenas a stack de observabilidade isoladamente, para testes locais, via `observability/docker-compose.yml` deste repositório.

## Repositórios relacionados

- 🔧 **Worker de processamento de doações**: [SCDonationProcessor](https://github.com/dairussi/SCDonationProcessor)
- ☁️ **Infraestrutura (Kubernetes local + AWS/EKS)**: [SolidarityConnectionDeployFile](https://github.com/dairussi/SolidarityConnectionDeployFile)
