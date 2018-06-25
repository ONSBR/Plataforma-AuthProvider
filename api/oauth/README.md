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


#### Url de Serviços

Url de autenticação: http://localhost:8085/OAuth/Token
Todo o processo de autenticação segui o padrão de protocolo OAuth.
Para mais detalhes sobre os parâmetros vide o OAuth.


#### Compilação 

PS > dotnet build 


#### Execução 

PS > dotnet .\bin\Debug\netcoreapp2.1\ONS.AuthProvider.OAuth.dll 8085


#### Configuração Factory

Configuração do adaptador de provedores de autenticação.

Definição: Auth:Server:AdapterUsage

Exemplo: 
  
  "Auth": {
      "Server": {
        "AdapterUsage": "Fake",
        "Adapters": {
          "Pop": {
              ...
          },
          "Fake": {
              ...
          }
        }
      }
    }


Default: Fake
Neste caso, será utilzado o adaptador Fake para configurar os providers deste mecanismo de autenticação.
Para configurar para utilizar os provedores do pop de autenticação, deve ser utilizado o adaptador do POP (Pop).


#### Configuração POP

Configuração do provider, onde é especificada a url de acesso ao POP.

  "Auth": {
      "Server": {
        "Adapters": {
          "Pop": {
              "Url.Jwt.OAuth": "https://poptst.ons.org.br/ons.pop.federation/oauth2/token",
          }
        }
      }
    }


#### Configuração Fake

O provider de autenticação Fake utiliza JWT para geração do token de autenticação. JWT: (https://jwt.io/)
Configuração do Provider de autenticação Fake. As configurações são para autenticação e geração do token JWT.

  "Auth": {
      "Server": {
        "Adapters": {
          "Fake": {
              "Type": "ONS.AuthProvider.OAuth.Providers.Fake.FakeAuthorizationAdapter",
              "AuthorizeEndpointPath": "/OAuth/Authorize",
              "TokenEndpointPath": "/OAuth/Token",
              "AllowInsecureHttp": "true",
              "AutomaticAuthenticate": "true",
              "Jwt.Username": "ONS\\user.pitang",
              "Jwt.Password": "RgcNS/k0+w1LBtnixG40aUTjIkRJAKQ119mpXm10NfU=",
              "Jwt.Token": {
                  "Key": "7/5jd7+Gwbq4S/l12uL7l8d345zpGbgg77QlpC6XmMo=",
                  "Audience": "INTANEEL,TESTE",
                  "Issuer": "https://poptst.ons.org.br/ons.pop.federation/",
                  "Role": "Servico,Servico2",
                  "Expiration.Minutes": 4,
                  "Refresh.Expiration.Minutes": 8
            }
          }
        }
      }
    }

As configurações de Jwt.Username e Jwt.Password, são dados de autenticação. Jwt.Token é para geração do Token de autenticação. A configuração de senha deverá ser criptografada em SHA256, neste caso, está com valor: "teste".


#### Exemplo de requisição de acesso aos serviços

Requisição de Autenticação:

    POST http://localhost:8085/OAuth/Token
    Content-Type: application/x-www-form-urlencoded
    Body: 
        username=ONS\user.pitang&password=teste&client_id=INTANEEL&grant_type=password

    
Requisição de Atualização do Token:

    POST http://localhost:8085/OAuth/Token
    Content-Type: application/x-www-form-urlencoded
    Body: 
        refresh_token=ayyK23J0Fey/XINmCuiO9knbL4rtnpKK5QC06FO7NGk=&client_id=INTANEEL&grant_type=refresh_token
        

Exemplo de resposta para solicitação de autenticação ou atualização:
    Body:    
        {
            "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6WyJ1c2VyIiwidXNlciJdLCJqdGkiOiI2Y2YwM2MwMzU2MDQ0NWQwODZkZGQ4NDBmYzg3ZTk2OSIsIm5iZiI6MTUyOTQzMjA1OCwiZXhwIjoxNTI5NDMyMTc4LCJpYXQiOjE1Mjk0MzIwNTgsImlzcyI6IkV4ZW1wbG9Jc3N1ZXIiLCJhdWQiOiJFeGVtcGxvQXVkaWVuY2UifQ.R8P4cNepLE542ma8sB0qxtZnREI6SQQlMaJIbVRSjhU",
            "token_type": "bearer",
            "expires_in": 1199,
            "refresh_token": "2a53d135c4e8483fb18530bd9b0fbb32"
        }

    