# FiapCloudGames.Payments

## 📌 Objetivos
Microsserviço de pagamentos do Monólito [FiapCloudGames](https://github.com/MarioGuilherme/FiapCloudGames) que trata todas as regras e lógicas pertinente ao escopo de pagamento dos pedidos de compras, juntamente com o sua base de dados e também simulando uma integração com o PagSeguro e envio de notificação via e-mail com o SendGrid.

## 🚀 Instruções de uso
Faça o clone do projeto e já acesse a pasta do projeto clonado:
```
git clone https://github.com/MarioGuilherme/FiapCloudGames.Payments && cd .\FiapCloudGames.Payments
```

### ▶️ Iniciar Projeto
  1 - Navegue até o diretório da camada API da aplicação:
  ```
  cd .\FiapCloudGames.Payments.API\
  ```
  2 - Insira o comando de execução do projeto:
  
  _(O BANCO DE DADOS É CRIADO AUTOMATICAMENTE QUANDO O PROJETO É INICIADO, SEM PRECISAR EXECUTAR O ```Database-Update```)_:
  ```
  dotnet run --launch-profile https
  ```
  3 - Acesse https://localhost:7190/swagger/index.html

### 🧪 Executar testes
  1 - Navegue até o diretório dos testes:
  ```
  cd .\FiapCloudGames.Payments.Tests\
  ```
  2 - E insira o comando de execução de testes:
  ```
  dotnet test
  ```

## 🛠️ Tecnologias e Afins
- .NET 8 com C# 12;
- ASP.NET Core;
- Logs Distribuídos com CorrelationId;
- Uso de Middlewares e IActionFilters;
- EntityFrameworkCore;
- Unit Of Work;
- SQL SERVER;
- FluentValidation;
- Swagger;
- xUnit junto com Moq;
- Autenticação JWT;
