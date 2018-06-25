using System;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ONS.AuthProvider.OAuth.Util.Http
{
    ///<summary>Classe responsável por registrar dados de requisições, executada pelo client Http.
    /// São registrados os dados de requisição e resposta da solicitação.</summary>
    public class LogHandler : DelegatingHandler
    {
        private readonly ILogger _logger;
        private KeyValuePair<string,string>[] _replacesContent;

        ///<summary>Construtor do Handle de log.</summary>
        public LogHandler(HttpMessageHandler innerHandler, ILogger logger, KeyValuePair<string,string>[] replaces = null)
            : base(innerHandler)
        {
            _logger = logger;
            _replacesContent = replaces;
        }

        private string _applyReplaceData(string content) {

            if (_replacesContent == null) return content;

            foreach (var item in _replacesContent) {
                content = Regex.Replace(content, item.Key, item.Value, RegexOptions.IgnoreCase);
            }
            return content;    
        }

        ///<summary>Método chamado quando é executada a requisição http do client.</summary>
        ///<param name="request">Requisição solicitada para execução.</param>
        ///<param name="cancellationToken">Cancelamento do token.</param>
        ///<returns>Resposta para a execução da requisição.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            if (_logger.IsEnabled(LogLevel.Trace)) 
            {
                var sb = new StringBuilder();
                sb.AppendLine("Request:");
                sb.AppendLine(request.ToString());
                if (request.Content != null)
                {
                    sb.AppendLine(await request.Content.ReadAsStringAsync());
                }
                sb.AppendLine();

                _logger.LogTrace(_applyReplaceData(sb.ToString()));
            }
                
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (_logger.IsEnabled(LogLevel.Trace)) 
            {
                var sb = new StringBuilder();
                sb.AppendLine("Response:");
                
                sb.AppendLine(response.ToString());
                if (response.Content != null)
                {
                    sb.AppendLine(await response.Content.ReadAsStringAsync());
                }
                sb.AppendLine();

                _logger.LogTrace(_applyReplaceData(sb.ToString()));
            }
            
            return response;
        }
    }
}