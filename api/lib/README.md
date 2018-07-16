# ONS.AuthProvider.Validator

#### Introdução
Biblioteca para configuração do controle de permissão, utilizando o autenticador da plataforma.


#### Requisitos

Para executar as aplicações com sucesso você precisa instalar as seguintes ferramentas:
* [.Net Core 2.1](https://www.microsoft.com/net/download/windows)


#### Configuração do Projeto

Para utilizar a biblioteca deve ser configurado no projeto, no Startup.cs de inicialização do sistema.

Exemplo de configuração:

using ONS.AuthProvider.Validator;
...
  public void ConfigureServices(IServiceCollection services)
  {
      ...
      AuthConfigurationValidate.Configure(services, new AuthValidateOptions{
          ValidIssuer = <issuer>,
          ValidAudience = <audience>,
          ValidKey = <key>,
          UseRsa = <useRsa>,
          FileRsaPublicKeyXml = <fileNameRsaPublicKeyXml>
      });
      ...
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
  }

  public void Configure(IApplicationBuilder app, IHostingEnvironment env)
  {
      ...
      app.UseAuthentication();
      ...
      app.UseMvc();
  }


#### Compilação 

PS > dotnet build 

