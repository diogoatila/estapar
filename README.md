# Estapar Backend Developer Test (.NET 8)

Backend em **.NET 8** para gerenciamento de estacionamento, com controle de vagas, entrada/saída de veículos, cálculo de cobrança e consulta de receita por setor/data.

---

## ✅ Stack

- **.NET 8 (C#)**
- **ASP.NET Core WebAPI**
- **Entity Framework Core**
- **SQL Server**
- **Swagger/OpenAPI**
- Arquitetura em camadas:
  - `Estapar.Api`
  - `Estapar.Application`
  - `Estapar.Domain`
  - `Estapar.Infrastructure`

---

## 📌 Requisitos implementados

### Funcionais
- Recebe eventos via **POST `/webhook`**:
  - `ENTRY`
  - `PARKED`
  - `EXIT`
- Consulta faturamento via **GET `/revenue`**
- Retorna configuração via **GET `/garage`**

### Regras de Negócio
- ENTRY ocupa uma vaga disponível e cria sessão ativa
- EXIT libera vaga e calcula cobrança
- **Primeiros 30 minutos grátis**
- Após 30 minutos:
  - tarifa por hora
  - arredondamento para cima (`Ceiling`)
  - valor base = `basePrice` do setor
- Bloqueia novas entradas quando o estacionamento estiver cheio (**HTTP 409**)
- Preço dinâmico calculado na entrada:
  - Lotação `< 25%` → desconto `-10%`
  - Lotação `<= 50%` → preço normal
  - Lotação `<= 75%` → acréscimo `+10%`
  - Lotação `<= 100%` → acréscimo `+25%`

---

## 🧠 Decisões técnicas importantes

### Idempotência
- ENTRY duplicado para a mesma placa não cria nova sessão.
- EXIT duplicado não gera cobrança extra.
- PARKED sem sessão ativa é ignorado (tolerante).

### Concorrência
- ENTRY e EXIT utilizam transação com **IsolationLevel.Serializable**
  para evitar corrida de ocupação/liberação de vagas.

### Índice filtrado (SQL Server)
Foi implementado índice único filtrado para garantir apenas **uma sessão ativa por placa**:

- `(LicensePlate)` UNIQUE
- FILTER: `ExitTime IS NULL`

---

## 🗄️ Banco de Dados

### Connection String (appsettings.json)

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=EstaparDb;Trusted_Connection=True;TrustServerCertificate=True"
}
