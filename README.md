# Plataforma-AuthProvider
Projeto de serviços de controle de acesso da plataforma


#### Estrutura dos Projetos
Projetos da solução:
    > api              // Projetos da Api
        > oauth        // Projeto de servidor de autenticação
        > lib          // Biblioteca para autenticação no client, com configuração de autorização
    > test             // Projetos de Testes
        > unit         // Projeto com testes unitários
        > web          // Projeto de teste com página de autenticação e testes simulando validação em uma aplicação
        > postman      // Arquivo json com a Collection de testes do servidor de autenticação


#### Documentação

api > oauth > Readme.md    // Orientações sobre o projeto do servidor de autenticação, geração de token.
api > lib > Readme.md      // Orientações sobre o projeto de biblioteca de validação do token de autenticação.


#### Testes do Servidor de Autenticação

Arquivo com as coleções de requisições de testes do postman:
    test > postman > Test_collection.json


#### Execução dos Testes de Autenticação

Execução do servidor de autenticação:
    PS > cd .\api\oauth\
    PS > dotnet build .\ONS.AuthProvider.OAuth.csproj
    PS > dotnet .\bin\Debug\netcoreapp2.1\ONS.AuthProvider.OAuth.dll 8085

Execução da aplicação do teste de autenticação:
    PS > cd .\test\web\
    PS > dotnet build .\ONS.AuthProvider.Test.Web.csproj
    PS > dotnet .\bin\Debug\netcoreapp2.1\ONS.AuthProvider.Test.Web.dll 8083

Execução de testes no postman:
    Autenticação:
        1. Executar: [POST] Auth Token
        2. Obter o Token da resposta, o "access_token"
        3. Substituir o valor do token do header no [POST] Test Protected
        4. Executar: [POST] Test Protected
    Atualização do Token:
        1. Executar: [POST] Auth Token
        2. Obter o Token da resposta, o "refresh_token"
        3. Substituir o valor do refresh token no body do [POST] Refresh Token
        4. Obter o novo Token da resposta, o "access_token"
        5. Executar: [POST] Test Protected        


#### Página de teste

No projeto de teste web está disponível uma página para acessar o servidor para geração de chave também.
Projeto: test > web

Para executar:
    PS > dotnet build .\ONS.AuthProvider.Test.Web.csproj
    PS > dotnet .\bin\Debug\netcoreapp2.1\ONS.AuthProvider.Test.Web.dll 8083

    Para acessar a página de teste: http://localhost:8083