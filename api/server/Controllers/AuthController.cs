using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ONS.AuthProvider.Api.Models;

using ONS.AuthProvider.Api.Services;
using ONS.AuthProvider.Api.Services.Impl.Pop;
using Newtonsoft.Json;

namespace ONS.AuthProvider.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        // TODO receber por client id
        private readonly IAuthServiceFactory _authServiceFactory;

        public AuthController(IAuthServiceFactory authServiceFactory) {
            _authServiceFactory = authServiceFactory;
        }

        [HttpGet]
        [Route("ToVivo")]
        public ActionResult<string> ToVivo()
        {
            return "Ok";
        }

        // POST api/values
        [HttpPost]
        [Route("token")]
        public ActionResult<Token> GenerateToken()
        {
            // TODO Poderia receber o user por bind...

            // TODO validar dados de entrada
            User user = _parseInputContent(() => {
                var _usr = new User();
                _usr.Username = Request.Form["username"];
                _usr.Password = Request.Form["password"];
                _usr.ClientId = Request.Form["clientid"];
                return _usr;
            });

            user.HostOrigin = Request.Headers["Referer"];

            Console.WriteLine("user: " + user);
            
            return this._authServiceFactory.Get(user.ClientId).Auth(user);
        }

        [HttpPost]
        [Route("refresh")]
        public ActionResult<Token> RefreshToken()
        {
            // TODO validar dados de entrada
            DataRefreshToken dataRefresh = _parseInputContent(() => {
                var _rfs = new DataRefreshToken();
                _rfs.ClientId = Request.Form["clientid"];
                _rfs.RefreshToken = Request.Form["refreshtoken"];
                return _rfs;
            });

            dataRefresh.HostOrigin = Request.Headers["Referer"];
            
            return this._authServiceFactory.Get(dataRefresh.ClientId).Refresh(dataRefresh);
        }

        private T _parseInputContent<T>(Func<T> actionAlternativeObj)
        {
            T retorno = default(T);

            if ("application/json".Equals(Request.ContentType))
            {
                using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                {  
                    string contentReq = reader.ReadToEndAsync().Result;
                    retorno = JsonConvert.DeserializeObject<T>(contentReq);
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
