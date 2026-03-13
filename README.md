# Users API - Desafio Final Grupo 118

API REST para gerenciamento de usuários, planos e associações de usuários a planos, desenvolvida em .NET 8 utilizando Arquitetura Hexagonal (Ports and Adapters).

## Para que serve

Esta API oferece funcionalidades para:
- **Autenticação e autorização** de usuários com JWT
- **Gerenciamento de usuários**: criação, atualização, exclusão e consulta
- **Gerenciamento de planos**: CRUD completo de planos de assinatura
- **Associação de usuários a planos**: vincular usuários a diferentes planos
- Cache distribuído com Redis para melhor performance
- Observabilidade com OpenTelemetry

## Pré-requisitos

### Ferramentas necessárias
- **.NET 8.0 SDK** ou superior
- **SQL Server** (para banco de dados)
- **Redis** (para cache distribuído)
- **Docker** (opcional, para containerização)

### Configuração do ambiente
Antes de executar a aplicação, configure as variáveis no `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "sua-connection-string-do-sql-server",
    "Redis": "localhost:6379"
  },
  "Security": {
    "SecurityKey": "sua-chave-de-seguranca",
    "JwtKey": "sua-chave-jwt",
    "JwtIssuer": "http://localhost:8080",
    "JwtAudience": "http://localhost:8080",
    "JwtExpirationMinutes": 60
  }
}
```

## Como desenvolver e colaborar

### 1. Clonar o repositório
```bash
git clone <url-do-repositorio>
cd final-challenge-grupo-118-users
```

### 2. Restaurar dependências
```bash
dotnet restore
```

### 3. Aplicar migrations do banco de dados
```bash
cd src/Adapters/Driving/FinalChallengeUsers.API
dotnet ef database update
```

### 4. Executar a aplicação
```bash
dotnet run --project src/Adapters/Driving/FinalChallengeUsers.API
```

A API estará disponível em `http://localhost:8080` e a documentação Swagger em `http://localhost:8080/swagger`.

### 5. Executar os testes
```bash
dotnet test
```

### Estrutura do projeto

O projeto segue a **Arquitetura Hexagonal**:
- **Core/Domain**: Entidades e interfaces (Ports)
- **Core/Application**: Regras de negócio e casos de uso
- **Adapters/Driving**: API REST (Controllers)
- **Adapters/Driven**: Implementações de infraestrutura (Database, Password)
- **tests/UnitTests**: Testes unitários

### Boas práticas para colaborar

1. **Crie uma branch** para cada feature/bugfix:
   ```bash
   git checkout -b feature/nome-da-feature
   ```

2. **Escreva testes** para novas funcionalidades

3. **Mantenha o código limpo** seguindo os padrões do projeto

4. **Faça commits descritivos** e atômicos

5. **Abra um Pull Request** com descrição clara das mudanças

### Docker

Para executar via Docker:
```bash
docker build -t users-api .
docker run -p 8080:8080 users-api
```

### Recursos adicionais

- Documentação da API: `/swagger`
- Collection do Postman: `FinalChallenge-Grupo118-Users.postman_collection.json`

