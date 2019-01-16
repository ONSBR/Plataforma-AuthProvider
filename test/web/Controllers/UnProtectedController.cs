using Microsoft.AspNetCore.Mvc;

namespace ONS.AuthProvider.Test.Web.Controllers
{
    [Route("api/unprotected")]
    [ApiController]
    public class UnProtectedController : ControllerBase
    {
        [Route("")]
        public string Get()
        {
            return "Ola";
        }
    }
}