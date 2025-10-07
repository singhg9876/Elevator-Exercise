using ElevatorSystem.Api.DTOs;
using Microsoft.Extensions.Options;

namespace ElevatorSystem.Api.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEY_HEADER_NAME = "Auth-Key";
        private readonly string _apiKey;
        public AuthenticationMiddleware(RequestDelegate next, IOptions<ApiSettings> options)
        {
            this._next = next;
            _apiKey = options.Value.ApiKey;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(APIKEY_HEADER_NAME, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API key is not found in the header");
                return;
            }
            if (!_apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Unauthorized Client");
                return;
            }
            await _next(context);
        }

    }
}