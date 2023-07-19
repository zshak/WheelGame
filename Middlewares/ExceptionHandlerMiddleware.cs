using System.Text.Json;
using System.Text;
using Wheel.Models;
using Wheel.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Wheel.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private RequestDelegate _next;
        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BaseRequestException e)
            {
                context.Response.StatusCode = e.statusCode;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(e.Message);
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync($"{e.Message}");
            }

        }
    }
}
