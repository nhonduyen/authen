using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace authen
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.Keys.Contains("X-Not-Authorized"))
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }
            if (!context.Request.Headers.Keys.Contains("Authorization"))
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }
            var token = context.Request.Headers["Authorization"];
            var validToken = new List<string>
            {
                "token1", "token2", "token3"
            };
            if (!validToken.Contains(token))
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }
            await _next.Invoke(context);
        }
    }
}