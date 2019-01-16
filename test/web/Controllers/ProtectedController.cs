using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ONS.AuthProvider.Test.Web.Controllers
{
    [Authorize(Roles = "Servico")]
    [Route("api/protected")]
    [ApiController]
    public class ProtectedController : ControllerBase
    {
        [Route("")]
        public IEnumerable<object> Get()
        {
            var identity = User.Identity as ClaimsIdentity;

            return identity.Claims.Select(c => new
            {
                c.Type,
                c.Value
            });
        }
    }
}