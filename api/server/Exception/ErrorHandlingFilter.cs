using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ONS.AuthProvider.Api.Exception
{

    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public ErrorHandlingFilter(ILogger<ErrorHandlingFilter> logger) {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            HandleExceptionAsync(context);
            context.ExceptionHandled = true;
        }

        private void HandleExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;

            _logger.LogError(exception.Message, exception);
            
            string msgException = "Erro inesperado.";
            int statusCode = StatusCodes.Status400BadRequest;

            if (exception is AuthException) {
                var authException = (AuthException) exception;
                msgException = authException.Message;
                if (authException.StatusCode.HasValue) {
                    statusCode = authException.StatusCode.Value;
                }
            }
            
            SetExceptionResult(context, msgException, statusCode);
        }

        private static void SetExceptionResult(
            ExceptionContext context, 
            string msgException, 
            int code)
        {
            context.Result = new JsonResult(new { error = msgException })
            {
                StatusCode = (int)code
            };
        }
    }

}