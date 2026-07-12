# FCGUser - Microsserviço de Gerenciamento de Usuários

## Descrição

O **FCGUser** é um microsserviço responsável pelo gerenciamento completo de usuários na plataforma **FIAP Cloud Games (FCG)**. Ele fornece funcionalidades de autenticação, autorização, cadastro de usuários, gerenciamento de perfis e publica eventos quando novos usuários são criados.

Este microsserviço implementa arquitetura em camadas (DDD - Domain Driven Design) com separação clara entre camadas de API, Aplicação, Domínio e Infraestrutura. Utiliza **JWT (JSON Web Tokens)** para autenticação e **RabbitMQ** para comunicação assíncrona com outros microsserviços.

---

## Funcionalidades Principais

- **Autenticação JWT**: Login seguro com tokens JWT com validade configurável
- **Registro de Usuários**: Cadastro de novos usuários com validação
- **Autorização baseada em Roles**: Suporte a roles (Admin, Common)
- **Gerenciamento de Status**: Usuários podem estar Active, Blocked ou Banned
- **Publicação de Eventos**: Publica `UserCreatedEvent` quando novo usuário é registrado
- **Integração com Banco de Dados**: Persistência em SQL Server
- **Documentação Swagger**: API totalmente documentada com autenticação JWT
- **Tratamento de Erros**: Middleware customizado para erros estruturados
- **Validação de Dados**: Validação automática com FluentValidation

---

## Arquitetura em Camadas

O projeto segue a arquitetura em camadas com Domain Driven Design:

```
┌─────────────────────────────────────────────────────────┐
│              UserAPI.API (Presentation)                 │
│  Controllers, Middlewares, Extensions                   │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│         UserAPI.Application (Business Logic)             │
│  Services, DTOs, Use Cases, Validators                  │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│         UserAPI.Domain (Business Rules)                  │
│  Entities, Interfaces, Enums, Events                    │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│       UserAPI.Infrastructure (Data Access)              │
│  Database, Messaging, Repository, Migrations            │
└─────────────────────────────────────────────────────────┘
```

---

## Dependências

- **Microsoft.AspNetCore.Authentication.JwtBearer**: Autenticação JWT
- **Swashbuckle.AspNetCore**: Swagger/OpenAPI
- **RabbitMQ.Client**: Cliente para RabbitMQ
- **Entity Framework Core**: ORM
- **FluentValidation**: Validação de regras de negócio

---

## Como Executar

### Pré-requisitos

- .NET 10.0 SDK ou superior instalado
- Docker e Docker Compose (para execução containerizada)
- SQL Server em execução (ou use o docker-compose fornecido)
- RabbitMQ em execução (ou use o docker-compose fornecido)

### Opção 1: Executar com Docker Compose

```bash
# Na raiz do projeto FCGUser (mesmo diretório deste README)
docker-compose up
```

Este comando inicia:
- **SQL Server** na porta 1433
- **RabbitMQ** na porta 5672 (AMQP) e 15672 (Management UI)
- **FCGUser API** na porta 8070

Verifique se os containers estão saudáveis:

```bash
docker-compose ps
```

---

### Opção 2: Executar Localmente

1. **Certifique-se de que SQL Server está em execução**:

```bash
# Use Docker apenas para SQL Server
docker run -d --name sqlserver \
  -e 'ACCEPT_EULA=Y' \
  -e 'MSSQL_SA_PASSWORD=Your_strong!Passw0rd' \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest
```

2. **Certifique-se de que RabbitMQ está em execução**:

```bash
# Use Docker para RabbitMQ
docker run -d --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:3-management
```

3. **Configure as variáveis de ambiente** em `UserAPI.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "ConnectionString": "Server=localhost,1433;Database=FiapCloudGames;User Id=sa;Password=Your_strong!Passw0rd;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "SecretKey": "_07P\\Wbk|}j:Bp-zC/%`701{Owu?B_dJLSDJaljdlajdlajlda",
    "Issuer": "FiapCloudGames",
    "Audience": "FiapCloudGames.API"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "QueueName": "user-created"
  },
  "CatalogApi": {
    "BaseUrl": "http://localhost:5137"
  }
}
```

4. **Restaure as dependências, aplique migrações e execute a API**:

```bash
cd UserAPI.API

# Restaurar dependências
dotnet restore

# Aplicar migrações de banco de dados
dotnet ef database update --project ../UserAPI.Infrastructure

# Executar a API
dotnet run
```

A API estará disponível em:
- **HTTP**: http://localhost:8070
- **Swagger**: http://localhost:8070/swagger

---

## Acessar a API

### Documentação Swagger com Autenticação JWT

1. Acesse http://localhost:8070/swagger
2. Clique no botão **"Authorize"** (cadeado no canto superior direito)
3. Insira um token JWT válido no formato: `Bearer <seu-token-aqui>`
4. Clique em "Authorize"
5. Agora todos os endpoints protegidos estarão acessíveis

### OpenAPI JSON

- **URL**: http://localhost:8070/swagger/v1/swagger.json

### RabbitMQ Management UI

- **URL**: http://localhost:15672
- **Usuário**: guest
- **Senha**: guest

---

## Autenticação JWT

### Estrutura do Token JWT

```
Header: {
  "alg": "HS256",
  "typ": "JWT"
}

Payload: {
  "sub": "user-id",
  "email": "user@example.com",
  "role": "Common",
  "iss": "FiapCloudGames",
  "aud": "FiapCloudGames.API",
  "exp": 1234567890,
  "iat": 1234567890
}

Signature: HMACSHA256(header + "." + payload, secret)
```

### Configuração JWT

Arquivo: `appsettings.json`

```json
"JwtSettings": {
  "SecretKey": "sua-chave-secreta-super-segura-aqui",
  "Issuer": "FiapCloudGames",
  "Audience": "FiapCloudGames.API"
}
```

**Variáveis de Ambiente**:

```bash
export JwtSettings__SecretKey=sua-chave-secreta
export JwtSettings__Issuer=FiapCloudGames
export JwtSettings__Audience=FiapCloudGames.API
```

---

## Eventos Publicados

### UserCreatedEvent

**Publicado quando**: Um novo usuário é registrado com sucesso

**Fila**: `user-created`

**Payload**:
```json
{
  "id": "uuid",
  "nome": "João Silva",
  "email": "joao@example.com"
}
```

**Consumidores**:
- **FCGNotification**: Envia email de boas-vindas
- **FCGCatalog**: Registra novo usuário no catálogo (opcional)

---

## Endpoints da API

### Documentação Completa

Para uma documentação interativa completa, acesse: http://localhost:8070/swagger

### Endpoints Principais

#### 1. Registrar Novo Usuário

```
POST /api/users/register
Content-Type: application/json

{
  "name": "João",
  "lastName": "Silva",
  "email": "joao@example.com",
  "password": "Password123!"
}
```

**Resposta (201 Created)**:
```json
{
  "id": "uuid",
  "name": "João",
  "lastName": "Silva",
  "email": "joao@example.com",
  "role": "Common",
  "status": "Active"
}
```

---

#### 2. Fazer Login

```
POST /api/users/login
Content-Type: application/json

{
  "email": "joao@example.com",
  "password": "Password123!"
}
```

**Resposta (200 OK)**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "user": {
    "id": "uuid",
    "name": "João",
    "email": "joao@example.com",
    "role": "Common"
  }
}
```

---

#### 3. Obter Perfil do Usuário

```
GET /api/users/profile
Authorization: Bearer <token-aqui>
```

**Resposta (200 OK)**:
```json
{
  "id": "uuid",
  "name": "João",
  "lastName": "Silva",
  "email": "joao@example.com",
  "role": "Common",
  "status": "Active",
  "createdAt": "2026-07-12T18:30:00Z"
}
```

---

#### 4. Atualizar Perfil

```
PUT /api/users/profile
Authorization: Bearer <token-aqui>
Content-Type: application/json

{
  "name": "João",
  "lastName": "Silva Oliveira"
}
```

---

#### 5. Listar Todos os Usuários (Admin Only)

```
GET /api/users
Authorization: Bearer <admin-token-aqui>
```

**Resposta (200 OK)**:
```json
[
  {
    "id": "uuid",
    "name": "João",
    "email": "joao@example.com",
    "role": "Common",
    "status": "Active"
  }
]
```

---

#### 6. Obter Usuário por ID (Admin Only)

```
GET /api/users/{id}
Authorization: Bearer <admin-token-aqui>
```

---

#### 7. Bloquear Usuário (Admin Only)

```
PUT /api/users/{id}/block
Authorization: Bearer <admin-token-aqui>
```

---

#### 8. Banir Usuário (Admin Only)

```
PUT /api/users/{id}/ban
Authorization: Bearer <admin-token-aqui>
```

---

## Roles e Permissões

### Roles Disponíveis

| Role | Descrição | Permissões |
|------|-----------|-----------|
| **Common** | Usuário comum | Visualizar próprio perfil, comprar jogos |
| **Admin** | Administrador | Gerenciar todos os usuários, modificar roles |

### Status do Usuário

| Status | Descrição | Pode fazer login? |
|--------|-----------|-------------------|
| **Active** | Usuário ativo e normal | Sim |
| **Blocked** | Usuário temporariamente bloqueado | Não |
| **Banned** | Usuário permanentemente banido | Não |

---

## Estrutura do Projeto

```
FCGUser/
├── UserAPI.API/                      # Camada de Apresentação
│   ├── Controllers/
│   │   └── UserController.cs         # Endpoints da API
│   ├── Extensions/                   # Configurações de extensão
│   ├── Middlewares/                  # Middleware customizado
│   ├── Program.cs                    # Configuração da aplicação
│   ├── UserAPI.API.csproj            # Arquivo de projeto
│   ├── appsettings.json              # Configurações
│   └── appsettings.Development.json  # Configurações de desenvolvimento
│
├── UserAPI.Application/              # Camada de Aplicação
│   ├── Services/
│   │   └── UserService.cs            # Serviços de negócio
│   ├── DTOs/
│   │   ├── Request/                  # DTOs de entrada
│   │   └── Response/                 # DTOs de saída
│   ├── Events/
│   │   └── UserCreatedEvent.cs       # Evento de domínio
│   ├── Validators/                   # Validações FluentValidation
│   └── ApplicationConfigModule.cs    # Configuração da camada
│
├── UserAPI.Domain/                   # Camada de Domínio
│   ├── Entities/
│   │   ├── BaseEntity.cs             # Entidade base
│   │   └── User.cs                   # Entidade User
│   ├── Enums/
│   │   ├── Role.cs                   # Enum de roles
│   │   └── Status.cs                 # Enum de status
│   ├── Interfaces/                   # Interfaces de contrato
│   ├── ExternalModels/               # Modelos externos
│   └── UserAPI.Domain.csproj         # Arquivo de projeto
│
├── UserAPI.Infrastructure/           # Camada de Infraestrutura
│   ├── Persistence/
│   │   ├── Configurations/           # Configurações EF Core
│   │   ├── Migrations/               # Migrações de banco
│   │   └── UserDbContext.cs          # DbContext
│   ├── Messaging/
│   │   └── RabbitMqPublisher.cs      # Publisher RabbitMQ
│   ├── Repository/                   # Repositories
│   ├── InfrastructureConfigModule.cs # Configuração da camada
│   └── UserAPI.Infrastructure.csproj # Arquivo de projeto
│
├── UserAPI.Tests/                    # Testes Unitários
│
├── Dockerfile                        # Build do container
├── docker-compose.yml                # Orquestração de containers
└── README.md                         # Este arquivo
```

---

## Configurações

### appsettings.json

```json
{
  "ConnectionStrings": {
    "ConnectionString": "Server=localhost,1433;Database=FiapCloudGames;User Id=sa;Password=Your_strong!Passw0rd;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "SecretKey": "_07P\\Wbk|}j:Bp-zC/%`701{Owu?B_dJLSDJaljdlajdlajlda",
    "Issuer": "FiapCloudGames",
    "Audience": "FiapCloudGames.API"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "QueueName": "user-created"
  },
  "CatalogApi": {
    "BaseUrl": "http://localhost:5137"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Variáveis de Ambiente

```bash
# Banco de Dados
export ConnectionStrings__ConnectionString=Server=localhost,1433;Database=FiapCloudGames;User Id=sa;Password=Your_strong!Passw0rd;TrustServerCertificate=True

# JWT
export JwtSettings__SecretKey=sua-chave-secreta-super-segura
export JwtSettings__Issuer=FiapCloudGames
export JwtSettings__Audience=FiapCloudGames.API

# RabbitMQ
export RabbitMQ__Host=localhost
export RabbitMQ__Port=5672
export RabbitMQ__Username=guest
export RabbitMQ__Password=guest
export RabbitMQ__QueueName=user-created

# API Externa
export CatalogApi__BaseUrl=http://localhost:5137
```

---

## Banco de Dados

### Esquema User

```sql
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Role INT NOT NULL DEFAULT 0, -- 0=Common, 1=Admin
    Status INT NOT NULL DEFAULT 0, -- 0=Active, 1=Blocked, 2=Banned
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL
);

CREATE INDEX IX_Email ON Users(Email);
```

### Executar Migrações

```bash
# Adicionar migração
dotnet ef migrations add MigrationName --project UserAPI.Infrastructure

# Aplicar migrações
dotnet ef database update --project UserAPI.Infrastructure

# Listar migrações
dotnet ef migrations list --project UserAPI.Infrastructure

# Reverter para uma migração anterior
dotnet ef database update PreviousMigrationName --project UserAPI.Infrastructure
```
---

## Fluxo de Registro e Autenticação

```
1. Usuário acessa a aplicação
   ↓
2. POST /api/users/register com nome, email e senha
   ↓
3. UserService valida e cria novo usuário
   ↓
4. User é salvo no banco de dados
   ↓
5. UserCreatedEvent é publicado no RabbitMQ
   ↓
6. FCGNotification consome e envia email
   ↓
7. Fluxo de registro concluído
   ↓
8. Usuário faz POST /api/users/login
   ↓
9. Credenciais são validadas
   ↓
10. JWT Token é gerado e retornado
   ↓
11. Usuário pode usar o token em requisições protegidas
```

---

##  Validações

### Validação de Registro

- **Nome**: 2-100 caracteres
- **Último Nome**: 2-100 caracteres
- **Email**: Formato válido de email, único na base
- **Senha**: Mínimo 8 caracteres, combinação de maiúscula, minúscula, número

### Validação de Login

- **Email**: Deve existir na base
- **Senha**: Deve corresponder ao hash armazenado

---

## Atualizar o Banco de Dados

```bash
# Ver dados atuais
sqlcmd -S localhost,1433 -U sa -P "Your_strong!Passw0rd"
> SELECT * FROM Users;

# Limpar dados (desenvolvimento apenas)
> DELETE FROM Users;

# Sair
> exit
```

---

## Relacionamento com Outros Microsserviços

### FCGNotification
- **Consome**: `UserCreatedEvent`
- **Uso**: Envia email de boas-vindas

### FCGCatalog
- **Usa**: Informações de usuário para contexto de compra
- **Consome**: `UserCreatedEvent` (opcional)

### FCGPayment
- **Usa**: ID de usuário para processar pagamentos

---

**Última atualização**: 2026-07-12  
**Versão**: 1.0.0 
