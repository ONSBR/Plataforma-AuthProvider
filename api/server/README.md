# ONS.AuthProvider.Api

#### Introdução
Aplicação de api de serviços de autenticação para a plataforma.
A api permite configurar o provedor de autenticação, por projeto. 
No projeto está disponível provedor de autenticação do POP do ONS. 
O POP é o provedor de autenticação padrão do ONS.
No projeto também está disponível um provedor Fake, de teste, configurado no "appsettings.json".

Na api são disponíveis serviços de autenticação (auth) e atualização do token (refresh).


#### Requisitos

Para executar as aplicações com sucesso você precisa instalar as seguintes ferramentas:
* [.Net Core 2.1](https://www.microsoft.com/net/download/windows)
* [Docker](https://www.docker.com/)


#### Serviços

Url de autenticação: https://localhost:5001/api/auth/token
Url de atualização da expiração do token: https://localhost:5001/api/auth/refresh


#### Configuração Factory

Configuração do Factory de provedor de autenticação.

Definição: Auth:Factory:Service.Alias<.clientId>
Obs: A opção de configuração sem o "clientId" é a configuração default, 
caso não tenha uma configuração específica para o "clientId".

Exemplo: 
  "Auth": {
      "Factory": {
        "Service.Alias": "jwt2",
        "Service.Alias.INTERNEL": "pop"
      }
  }


Default: jwt2
Para o projeto com clientId igual a INTERNEL, será utilizado o provider com álias "pop".
Para o projeto com clientId igual a TESTE, será utilizado o provider com álias "jwt2", pois como não foi especificado, é utilizado o default.


#### Configuração POP

Configuração do provider, onde é especificada a url de acesso ao POP.

  "Auth": {
      "Pop": {
        "Url.Jwt.OAuth": "https://poptst.ons.org.br/ons.pop.federation/oauth2/token"
      }
  }


#### Configuração Fake

O provider de autenticação Fake utiliza JWT para geração do token de autenticação. JWT: (https://jwt.io/)
Configuração do Provider de autenticação Fake. As configurações são para autenticação e geração do token JWT.

  "Auth": {
      "Fake": {
        "Jwt.Username": "user",
        "Jwt.Password": "46070d4bf934fb0d4b06d9e2c46e346944e322444900a435d7d9a95e6d7435f5",
        "Jwt.Token": {
          "Key": "veryVerySecretKey",
          "Audience": "ExemploAudience",
          "Issuer": "ExemploIssuer",
          "Expiration.Seconds": 120
        }
      }
  }

As configurações de Jwt.Username e Jwt.Password, são dados de autenticação. Jwt.Token é para geração do Token de autenticação. A configuração de senha deverá ser criptografada em SHA256, neste caso, está co valor: "teste".


#### Exemplo de requisição de acesso aos serviços

Requisição de Autenticação:

    POST https://localhost:5001/api/auth/token 
    Content-Type: application/json
    Body:
        {
            "Username": "user", 
            "Password": "teste", 
            "ClientId": "INTANEEL"
        }

Requisição de Atualização do Token:

    POST https://localhost:5001/api/auth/refresh 
    Content-Type: application/json
    Body:
        {
            "RefreshToken": "ayyK23J0Fey/XINmCuiO9knbL4rtnpKK5QC06FO7NGk=", 
            "ClientId": "INTANEEL"
        }    

Exemplo de resposta para solicitação de autenticação ou atualização:
    Body:    
        {
            "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6WyJ1c2VyIiwidXNlciJdLCJqdGkiOiI2Y2YwM2MwMzU2MDQ0NWQwODZkZGQ4NDBmYzg3ZTk2OSIsIm5iZiI6MTUyOTQzMjA1OCwiZXhwIjoxNTI5NDMyMTc4LCJpYXQiOjE1Mjk0MzIwNTgsImlzcyI6IkV4ZW1wbG9Jc3N1ZXIiLCJhdWQiOiJFeGVtcGxvQXVkaWVuY2UifQ.R8P4cNepLE542ma8sB0qxtZnREI6SQQlMaJIbVRSjhU",
            "tokenType": "bearer",
            "expiresIn": 1199,
            "refreshToken": "2a53d135c4e8483fb18530bd9b0fbb32"
        }

OBS: Nas requisições de autenticação ou atualização podem ser utilizados parâmetros de formulário ao invés do json.
Exemplo: 
    POST https://localhost:5001/api/auth/token
    Content-Type: application/x-www-form-urlencoded
    Body: Username=user&Password=teste&ClientId=INTANEEL
    