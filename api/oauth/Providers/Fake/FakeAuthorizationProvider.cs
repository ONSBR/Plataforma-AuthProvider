using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OAuth.AspNet.AuthServer;
using System.Security.Claims;
using System.Security.Principal;


using System.Collections.Concurrent;

namespace ONS.AuthProvider.OAuth.Providers.Fake
{
    public class FakeAuthorizationProvider 
    {
        private readonly ConcurrentDictionary<string, string> _authenticationCodes = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        public Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            Console.WriteLine("###### 1 >>>");
            context.Validated();
            
            return Task.FromResult(0);
        }

        public Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            Console.WriteLine("###### 2 >>>");
            string clientId;
            string clientSecret;
            //if (context.TryGetBasicCredentials(out clientId, out clientSecret) ||
            //    context.TryGetFormCredentials(out clientId, out clientSecret))
            //{
                context.Validated();
            //}
            return Task.FromResult(0);
        }

        public Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            Console.WriteLine("###### 3 >>>");
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new GenericIdentity(context.UserName, OAuthDefaults.AuthenticationType), context.Scope.Select(x => new Claim("urn:oauth:scope", x))));

Console.WriteLine("###### principal >>>: " + principal);
            context.Validated(principal);

            return Task.FromResult(0);
        }

        public Task GrantClientCredetails(OAuthGrantClientCredentialsContext context)
        {
            Console.WriteLine("###### 4 >>>");
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new GenericIdentity(context.ClientId, OAuthDefaults.AuthenticationType), context.Scope.Select(x => new Claim("urn:oauth:scope", x))));

            context.Validated(principal);

            return Task.FromResult(0);
        }

        public void CreateAuthenticationCode(AuthenticationTokenCreateContext context)
        {
            Console.WriteLine("###### 5 >>>");
            context.SetToken(// TODO context.UserName
                FakeJwtTokenGenerator.GenerateToken("ONS\\lucilio.pitang")
            );
            Console.WriteLine("token:" + FakeJwtTokenGenerator.GenerateToken("ONS\\lucilio.pitang"));
            //context.SetToken(Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n"));
            _authenticationCodes[context.Token] = context.SerializeTicket();
        }

        public void ReceiveAuthenticationCode(AuthenticationTokenReceiveContext context)
        {
            string value;
            if (_authenticationCodes.TryRemove(context.Token, out value))
            {
                context.DeserializeTicket(value);
            }
        }

        public void CreateRefreshToken(AuthenticationTokenCreateContext context)
        {
            context.SetToken(context.SerializeTicket());
        }

        public void ReceiveRefreshToken(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
        }

    }
}