using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;

using ONS.AuthProvider.OAuth.Util.Http;
using ONS.AuthProvider.OAuth.Util;

using Microsoft.AspNetCore.Http;

namespace ONS.AuthProvider.OAuth.Providers.Pop
{
    /// <summary>Serviço responsável por realizar as chamadas para autenticação no Pop.
    /// Com a tecnologia JWT é gerado um Token de validade de acesso para uso do client.</summary>
    public class PopAuthJwtService 
    {
        private const string KeyConfigPathUrlPop = "Auth:Server:Adapters:Pop:Url.Jwt.OAuth";
        private const string KeyConfigHeaderReferer = "Auth:Server:Adapters:Pop:Header.Referer.Default";     

        private readonly ILogger _logger;
        private KeyValuePair<string,string>[] _replacesContent;

        public PopAuthJwtService() {

            _logger = AuthLoggerFactory.Get<PopAuthJwtService>();
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

        public PopToken RequestOAuth(IDictionary<string, string> parameters, string hostOrigin = null)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            foreach(var param in parameters) {
                query[param.Key] = param.Value;
            }
            
            return _processAuthPop(query, hostOrigin);
        }
        
        private PopToken _processAuthPop(NameValueCollection query, string hostOrigin = null)
        {
            string url = null;
            HttpResponseMessage response = null;
            JObject responseObjectJson = null;
            try {
                var watch = new Stopwatch();
                watch.Start();

                url = AuthConfiguration.Get(KeyConfigPathUrlPop);

                using (HttpClient client = CreateHttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    if (string.IsNullOrEmpty(hostOrigin)) {
                        hostOrigin = AuthConfiguration.Get(KeyConfigHeaderReferer);
                    }

                    if (!string.IsNullOrEmpty(hostOrigin)) {
                        client.DefaultRequestHeaders.Add("Referer", hostOrigin);
                    }

                    string queryString = query.ToString();

                    var strContent = new StringContent(
                        queryString, System.Text.Encoding.UTF8, 
                        "application/x-www-form-urlencoded"
                    );
                    response = client.PostAsync("", strContent).Result;

                    // Deserialize the updated product from the response body.
                    var responseContentString = response.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(responseContentString)) {
                        responseObjectJson = (JObject)JsonConvert.DeserializeObject(responseContentString);
                    }
                    
                    response.EnsureSuccessStatusCode();

                    var retorno = new PopToken();
                    retorno.AccessToken = responseObjectJson.Value<string>(Constants.Parameters.AccessToken);
                    retorno.TokenType = responseObjectJson.Value<string>(Constants.Parameters.TokenType);
                    retorno.ExpiresIn = responseObjectJson.Value<long>(Constants.Parameters.ExpiresIn);
                    retorno.RefreshToken = responseObjectJson.Value<string>(Constants.Parameters.RefreshToken);

                    if (_logger.IsEnabled(LogLevel.Debug)) {
                    
                        var msg = string.Format(
                            "Autenticação no pop realizada com sucesso. Data: grant_type={0}, client_id={1}, username={2}, refresh_token={3}." +
                            "Tempo[{4}ms]", 
                            query["grant_type"], query["client_id"], query["username"], query["refresh_token"],
                            watch.ElapsedMilliseconds
                        );
                        
                        _logger.LogDebug(msg);
                    }

                    return retorno;
                }

            } catch(System.Exception ex) {
                
                var msg = string.Format("Erro ao acessar o Pop[url={0}]", url);
                _logger.LogError(msg, ex);
                
                string error = Constants.Errors.InvalidRequest;
                string errorDescription = "Unexpect error";
                
                // Deserialize the updated product from the response body.
                if (responseObjectJson != null) {
                    
                    error = responseObjectJson.Value<string>(Constants.Parameters.Error);
                    errorDescription = responseObjectJson.Value<string>(Constants.Parameters.ErrorDescription);
                }

                throw new PopException(error, errorDescription);
            }
        }
        
    }
}
