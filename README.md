# Plataforma-AuthProvider
Projeto de serviços de controle de acesso da plataforma


#### Estrutura dos Projetos
Projetos da solução:
    > api              // Projetos da Api
        > server       // Projeto de servidor de autenticação
        > lib          // Biblioteca para autenticação no client, com configuração de autorização
    > test             // Projetos de Testes
        > app          // Projeto de Teste simulando validação em uma aplicação
        > unit         // Projeto com testes unitários
        > web          // Projeto de teste com página de autenticação
        > postman      // Arquivo json com a Collection de testes do servidor de autenticação


#### Documentação

api > server > Readme.md    // Orientações sobre o projeto do servidor de autenticação, geração de token.
api > lib > Readme.md       // Orientações sobre o projeto de biblioteca de validação do token de autenticação.


#### Testes do Servidor de Autenticação

Arquivo com as coleções de requisições de testes do postman:
    test > postman > Test_collection.json


#### Execução dos Testes de Autenticação

Execução do servidor de autenticação:
    PS > cd .\api\server\
    PS > dotnet build .\ONS.AuthProvider.Api.csproj
    PS > dotnet .\bin\Debug\netcoreapp2.1\ONS.AuthProvider.Api.dll 8082

Execução da aplicação do teste de autenticação:
    PS > cd .\test\app\
    PS > dotnet build .\ONS.AuthProvider.AppTest.csproj
    PS > dotnet .\bin\Debug\netcoreapp2.1\ONS.AuthProvider.AppTest.dll 8083

Execução de testes no postman:
    Autenticação:
        1. Executar: [POST] Auth Token
        2. Obter o Token da resposta, o "access_token"
        3. Substituir o valor do token do header no [POST] Test Fake Protected
        4. Executar: [POST] Test Fake Protected
    Atualização do Token:
        1. Executar: [POST] Auth Token
        2. Obter o Token da resposta, o "refresh_token"
        3. Substituir o valor do refresh token no body do [POST] Refresh Token
        4. Obter o novo Token da resposta, o "access_token"
        5. Executar: [POST] Test Fake Protected        

