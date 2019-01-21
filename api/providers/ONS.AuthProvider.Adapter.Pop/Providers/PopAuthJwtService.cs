using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONS.AuthProvider.Common.Providers;
using ONS.AuthProvider.Common.Util;
using ONS.AuthProvider.Common.Util.Http;

namespace ONS.AuthProvider.Adapter.Pop.Providers
{
    /// <summary>
    ///     Serviço responsável por realizar as chamadas para autenticação no Pop.
    ///     Com a tecnologia JWT é gerado um Token de validade de acesso para uso do client.
    /// </summary>
    public class PopAuthJwtService
    {
        private readonly JwtToken _configToken;
        private readonly ILogger _logger;
        private readonly KeyValuePair<string, string>[] _replacesContent;

        public PopAuthJwtService(JwtToken configToken)
        {
            _logger = AuthLoggerFactory.Get<PopAuthJwtService>();
            _configToken = configToken;
            _replacesContent = new[]
            {
                new KeyValuePair<string, string>("[&]password[=][^&]*[&]", "&password=XXXXX&")
            };
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient(new LogHandler(new HttpClientHandler(), _logger, _replacesContent));

            // Update port # in the following line.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );

            return client;
        }

        public PopToken RequestOAuth(IDictionary<string, string> parameters, string hostOrigin, string cookiesValue)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            foreach (var param in parameters) query[param.Key] = param.Value;

            return _processAuthPop(query, hostOrigin, cookiesValue);
        }

        private PopToken _processAuthPop(NameValueCollection query, string hostOrigin, string cookiesValue)
        {
            string url = null;
            HttpResponseMessage response = null;
            JObject responseObjectJson = null;
            try
            {
                var watch = new Stopwatch();
                watch.Start();

                url = _configToken.UrlJwtOAuth;

                using (var client = CreateHttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    if (string.IsNullOrEmpty(hostOrigin)) hostOrigin = _configToken.HeaderRefererDefault;

                    if (!string.IsNullOrEmpty(hostOrigin)) client.DefaultRequestHeaders.Add("Origin", hostOrigin);

                    if (!string.IsNullOrEmpty(cookiesValue)) client.DefaultRequestHeaders.Add("Cookie", cookiesValue);

                    var queryString = query.ToString();

                    var strContent = new StringContent(
                        queryString, Encoding.UTF8,
                        "application/x-www-form-urlencoded"
                    );
                    response = client.PostAsync("", strContent).Result;

                    // Deserialize the updated product from the response body.
                    var responseContentString = response.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(responseContentString))
                        responseObjectJson = (JObject) JsonConvert.DeserializeObject(responseContentString);

                    response.EnsureSuccessStatusCode();

                    var retorno = new PopToken();
                    retorno.AccessToken = responseObjectJson.Value<string>(Constants.Parameters.AccessToken);
                    retorno.TokenType = responseObjectJson.Value<string>(Constants.Parameters.TokenType);
                    retorno.ExpiresIn = responseObjectJson.Value<long>(Constants.Parameters.ExpiresIn);
                    retorno.RefreshToken = responseObjectJson.Value<string>(Constants.Parameters.RefreshToken);

                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
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
            }
            catch (Exception ex)
            {
                var msg = string.Format("Erro ao acessar o Pop[url={0}]", url);
                _logger.LogError(msg, ex);

                var error = Constants.Errors.InvalidRequest;
                var errorDescription = "Unexpect error";

                // Deserialize the updated product from the response body.
                if (responseObjectJson != null)
                {
                    error = responseObjectJson.Value<string>(Constants.Parameters.Error);
                    errorDescription = responseObjectJson.Value<string>(Constants.Parameters.ErrorDescription);
                }

                throw new PopException(error, errorDescription);
            }
        }
    }
}