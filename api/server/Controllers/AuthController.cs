using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ONS.AuthProvider.Api.Models;

using ONS.AuthProvider.Api.Services;
using ONS.AuthProvider.Api.Exception;
using ONS.AuthProvider.Api.Services.Impl.Pop;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace ONS.AuthProvider.Api.Controllers
{
    ///<summary>Controller para receber a requisição de autenticação para a plataforma ONS.</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger _logger;

        ///<summary>Factory de obtenção de serviços.</summary>
        private readonly IAuthServiceFactory _authServiceFactory;

        public AuthController(IAuthServiceFactory authServiceFactory, ILogger<AuthController> logger) {
            _logger = logger;
            _authServiceFactory = authServiceFactory;
        }

        [HttpGet]
        [Route("tovivo")]
        public ActionResult<string> ToVivo()
        {
            return "Ok";
        }

        /// <summary>Método responsável por fazer a autenticação e geração no Token de validação dos dados de autenticação.
        /// Esse token possui uma validade.</summary>
        [HttpPost]
        [Route("token")]
        public ActionResult<Token> GenerateToken()
        {
            Stopwatch watch = null;
            if (_logger.IsEnabled(LogLevel.Debug)) {

                watch = new Stopwatch();
                watch.Start();

                _logger.LogDebug("Recebida requisição de autenticação.");
            }            

            User user = _parseInputContent(() => {
                var _usr = new User();
                _usr.Username = Request.Form["Username"];
                _usr.Password = Request.Form["Password"];
                _usr.ClientId = Request.Form["ClientId"];
                return _usr;
            });

            if (_logger.IsEnabled(LogLevel.Debug)) {
                _logger.LogDebug(string.Format("Dados de autenticação recebidos com sucesso. User: {0}", user));
            }

            user.Validate();

            user.HostOrigin = Request.Headers["Referer"];

            var result = this._authServiceFactory.Get(user.ClientId).Auth(user);

            if (_logger.IsEnabled(LogLevel.Debug)) {
                var msg = string.Format(
                    "Autenticação realizada com sucesso. User: {0}. Tempo[{1}ms]", 
                    user,
                    watch.ElapsedMilliseconds
                );
                
                _logger.LogDebug(msg);
            }

            return result;
        }

        /// <summary>Método responsável por fazer a atualização do token de atualização do cliente.
        /// Esse token possui uma validade.</summary>
        [HttpPost]
        [Route("refresh")]
        public ActionResult<Token> RefreshToken()
        {
            Stopwatch watch = null;
            if (_logger.IsEnabled(LogLevel.Debug)) {

                watch = new Stopwatch();
                watch.Start();

                _logger.LogDebug("Recebida requisição de atualização do token de autenticação.");
            }
            
            DataRefreshToken dataRefresh = _parseInputContent(() => {
                var _rfs = new DataRefreshToken();
                _rfs.ClientId = Request.Form["ClientId"];
                _rfs.RefreshToken = Request.Form["RefreshToken"];
                return _rfs;
            });

            if (_logger.IsEnabled(LogLevel.Debug)) {
                _logger.LogDebug(string.Format(
                    "Dados de atualização do token recebidos com sucesso. Refresh: {0}", 
                    dataRefresh
                ));
            }

            dataRefresh.Validate();

            dataRefresh.HostOrigin = Request.Headers["Referer"];
            
            var result = this._authServiceFactory.Get(dataRefresh.ClientId).Refresh(dataRefresh);

            if (_logger.IsEnabled(LogLevel.Debug)) {
                var msg = string.Format(
                    "Atualização do token realizada com sucesso. Refresh: {0}. Tempo[{1}ms]", 
                    dataRefresh,
                    watch.ElapsedMilliseconds
                );
                
                _logger.LogDebug(msg);
            }

            return result;
        }

        /// <summary>
        /// Realiza a transformação dos dados obtidos do request. 
        /// Pode obter por json ou formulário, de acordo com o content-type. 
        /// </summary>
        /// <param name="actionAlternativeObj">Executa código de como deve ser feita a 
        /// transformação para receber os dados de formulário.</param>
        private T _parseInputContent<T>(Func<T> actionAlternativeObj)
        {
            T retorno = default(T);

            if ("application/json".Equals(Request.ContentType))
            {
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {  
                    string contentReq = reader.ReadToEndAsync().Result;
                    if (!string.IsNullOrEmpty(contentReq)) {
                        try {
                            retorno = JsonConvert.DeserializeObject<T>(contentReq);
                        } catch(System.Exception ex) {
                            throw new AuthException("Content json invalid.", ex, StatusCodes.Status400BadRequest);    
                        }
                    } else {
                        throw new AuthException("Content json not informed.", StatusCodes.Status400BadRequest);
                    }
                }   
            }
            else
            {
                retorno = actionAlternativeObj.Invoke();
            }

            return retorno;
        }
    }
}
