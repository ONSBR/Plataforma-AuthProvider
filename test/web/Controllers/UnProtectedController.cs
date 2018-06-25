using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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