# Desafio Umbler

Aplicação web que recebe um domínio e exibe suas informações de DNS, consultando servidores DNS e WHOIS.

## Como rodar o projeto

### Pré-requisitos

- .NET 6 SDK
- Node.js
- MySQL 8.0

### Configuração do banco de dados

Ajuste a connection string no `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=umbler;user=root;password=suasenha"
}
```

### Comandos
```bash
# Instalar dependências e buildar o frontend
npm install
npm run build

# Executar a migration
dotnet tool update --global dotnet-ef --version 6.0.0
dotnet ef database update

# Rodar o projeto
dotnet run
```

---

## O que foi mudado e por quê

### Problema de ambiente

Ao tentar rodar o projeto pela primeira vez, dois problemas impediram a execução:

- O comando `dotnet tool update --global dotnet-ef` instalava a versão 10, incompatível com .NET 6. Fixei a versão em `6.0.0`.
- A connection string usava `uid=root` mas o driver MySQL para .NET espera `user=root`. Corrigi o formato.

---

### Backend

**Controller estava fazendo trabalho demais**

O `DomainController` original tinha toda a lógica de negócio misturada com infraestrutura, consultas ao WHOIS, DNS e banco de dados tudo no mesmo método. Além disso, o mesmo bloco de código aparecia duas vezes (uma para domínio novo, outra para domínio expirado).

Extraí essa lógica para um `DomainService` e criei a interface `IDomainService`. O controller agora só valida a entrada e delega pro service.

**API expunha dados internos desnecessariamente**

O endpoint retornava a entidade `Domain` direto, incluindo `id`, `ttl`, `updatedAt` e o texto bruto do WHOIS. Criei um `DomainViewModel` com só o que o frontend precisa: `name`, `ip`, `hostedAt` e `nameServers`. Os nameservers são parseados do texto WHOIS e retornados como lista.

**Sem validação**

Qualquer string chegava no controller e causava exception. Adicionei uma validação simples: domínio vazio ou sem ponto retorna 400.

---

### Testes

O teste `Domain_Moking_WhoisClient` estava comentado com um TODO justamente porque era impossível testar o `WhoisClient` era estático e instanciado diretamente no controller, sem como mockar.

Com a extração para o `DomainService` e a interface `IDomainService`, todos os testes passam usando Moq, sem chamadas externas e sem banco de dados real. Também adicionei um teste para cobrir o caso de domínio inválido retornando 400.

---

### Frontend

O frontend original usava vanilla JS e exibia o JSON cru na tela, sem formatação e sem nenhuma validação antes de chamar o servidor.

Migrei para React aproveitando o Webpack e Babel que já estavam configurados no projeto. Precisei adicionar `babel-preset-react` e `babel-plugin-transform-class-properties` para suporte a JSX, e atualizar a chamada do ReactDOM para `createRoot` (obrigatório no React 18+).

O resultado agora mostra IP, empresa hospedeira e nameservers de forma organizada, valida o domínio antes de submeter e exibe feedback de carregamento enquanto a consulta acontece.