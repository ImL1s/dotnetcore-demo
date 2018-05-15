using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace dotnetcore_demo
{
    public class SecondMiddleware
    {
        private readonly RequestDelegate _next;

        public SecondMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await context.Response.WriteAsync($"{nameof(SecondMiddleware)} in. \r\n");
            await _next(context);
            await context.Response.WriteAsync($"{nameof(SecondMiddleware)} out. \r\n");
        }
    }
}