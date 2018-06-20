# ONS.AuthProvider.Validator

#### Introdução
Biblioteca para configuração do controle de permissão, utilizando o autenticador da plataforma.

#### Requisitos

Para executar as aplicações com sucesso você precisa instalar as seguintes ferramentas:
* [.Net Core 2.1](https://www.microsoft.com/net/download/windows)


#### Configuração do Projeto
Para utilizar a biblioteca deve ser configurado no projeto, no appsettings.json, 
os seguintes parâmetros:

  "Auth": {
    "Validate": {
      "Issuer": "https://hompops.ons.org.br/ons.pop.federation/",
      "Audience": "SCPCB",
      "Key": "ToxmY9olPvq7LpRaQU89qZynUQETYlNeFkEGG1ohEzA="
    }
  }


#### Compilação 

PS > dotnet build 

