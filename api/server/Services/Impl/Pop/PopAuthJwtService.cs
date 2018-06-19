using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using ONS.AuthProvider.Api.Models;
using ONS.AuthProvider.Api.Services;
using ONS.AuthProvider.Api.Exception;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;

using ONS.AuthProvider.Api.Utils.Http;

using Microsoft.AspNetCore.Http;

namespace ONS.AuthProvider.Api.Services.Impl.Pop
{
    /// <summary>Serviço responsável por realizar as chamadas para autenticação no Pop.
    /// Com a tecnologia JWT é gerado um Token de validade de acesso para uso do client.</summary>
    public class PopAuthJwtService : IAuthService
    {
        private const string ConfigPathUrlPop = "Auth:Pop:Url.Jwt.OAuth";

        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private KeyValuePair<string,string>[] _replacesContent;

        public PopAuthJwtService(IConfiguration configuration, ILogger<PopAuthJwtService> logger) {

            _configuration = configuration;
            _logger = logger;
            _replacesContent = new KeyValuePair<string,string>[] {
                new KeyValuePair<string,string>("[&]password[=][^&]*[&]", "&password=XXXXX&")
            };
        }

        private HttpClient CreateHttpClient() {

            HttpClient client = new HttpClient(new LogHandler(new HttpClientHandler(), _logger, _replacesContent));

            // Update port # in the following line.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );

            return client;
        }

        /// <summary>Autenticação do usuário e geração do token de validade, com expiração.</summary>
        /// <param name="user">Dados do usuário para autenticação.</param>
        /// <returns>Dados do token de autenticação.</returns>
        public Token Auth(User user)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["username"] = user.Username;
            query["password"] = user.Password;
            query["client_id"] = user.ClientId;
            query["grant_type"] = "password";

            return _processAuthPop(query, user.ClientId, user.HostOrigin);
        }

        /// <summary>Token com expiração atualizada, de validade de autenticação.</summary>
        /// <param name="dataRefresh">Dados para atualização do token.</param>
        /// <returns>Dados do token atualizado para nova expiração.</returns>
        public Token Refresh(DataRefreshToken dataRefresh) 
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["refresh_token"] = dataRefresh.RefreshToken;
            query["client_id"] = dataRefresh.ClientId;
            query["grant_type"] = "refresh_token";

            return _processAuthPop(query, dataRefresh.ClientId, dataRefresh.HostOrigin);
        }

        private void _validateUrlAuthPop(string urlAuthJwtPop) 
        {
            if (string.IsNullOrEmpty(urlAuthJwtPop)) {
                
                var msg = string.Format(@"Url de acesso ao serviço de geração de token do Pop não encontrada. 
                    Vide appsettings<.[env]>.json, path: {0}.<clientId> ", ConfigPathUrlPop);

                _logger.LogError(msg);

                throw new AuthException("Configuration internal error.", StatusCodes.Status500InternalServerError);
            }
        }

        private string _getConfigPathUrlPop(string clientId) 
        {
            var retorno = _configuration[ConfigPathUrlPop];
            
            if (!string.IsNullOrEmpty(clientId)) {
              var url = _configuration[string.Format("{0}.{1}", ConfigPathUrlPop, clientId)];
              if (!string.IsNullOrEmpty(url)) { 
                  retorno = url;
              }
            } 
            
            _validateUrlAuthPop(retorno);

            return retorno;
        }

        private Token _processAuthPop(NameValueCollection query, string clientId, string hostOrigin)
        {
            string url = null;
            HttpResponseMessage response = null;
            try {
                var watch = new Stopwatch();
                watch.Start();

                url = _getConfigPathUrlPop(clientId);

                using (HttpClient client = CreateHttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    if (!string.IsNullOrEmpty(hostOrigin)) {
                        client.DefaultRequestHeaders.Add("Referer", hostOrigin);
                    }

                    string queryString = query.ToString();

                    response = client.PostAsync("", new StringContent(queryString)).Result;

                    response.EnsureSuccessStatusCode();

                    // Deserialize the updated product from the response body.
                    var contentString = response.Content.ReadAsStringAsync().Result;
                    var jsobj = (JObject)JsonConvert.DeserializeObject(contentString);

                    var retorno = new Token();
                    retorno.AccessToken = jsobj.Value<string>("access_token");
                    retorno.TokenType = jsobj.Value<string>("token_type");
                    retorno.ExpiresIn = jsobj.Value<long>("expires_in");
                    retorno.RefreshToken = jsobj.Value<string>("refresh_token");

                    return retorno;
                }

                if (_logger.IsEnabled(LogLevel.Debug)) {
                    
                    var msg = string.Format(
                        "Autenticação no pop realizada com sucesso. Data: grant_type={0}, client_id={1}, username={2}, refresh_token={3}." +
                        "Tempo[{4}ms]", 
                        query["grant_type"], query["client_id"], query["username"], query["refresh_token"],
                        watch.ElapsedMilliseconds
                    );
                    
                    _logger.LogDebug(msg);
                }

            } catch(System.Exception ex) {
                var msg = string.Format("Erro ao acessar o Pop[url={0}]", url);
                _logger.LogError(msg, ex);
                // TODO tratamento exceção
                throw;
            }
        }
        
    }
}
