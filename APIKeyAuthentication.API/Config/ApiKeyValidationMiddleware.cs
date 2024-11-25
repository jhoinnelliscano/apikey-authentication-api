using System.Linq;

namespace APIKeyAuthentication.API.Config
{
    public class ApiKeyValidationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        private readonly RequestDelegate _next = next;
        private readonly IConfiguration _configuration = configuration;

        public async Task InvokeAsync(HttpContext context)
        {
            // Buscar la API Key en los encabezados
            if (!context.Request.Headers.TryGetValue("x-api-key", out var extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("API Key is missing");
                return;
            }

            // Obtener las claves válidas desde la configuración
            var validApiKeys = _configuration.GetSection("ApiKeys").Get<string[]>();

            if (validApiKeys!= null && !validApiKeys.Any(x=>x.Equals(extractedApiKey)))
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            // Continuar con la solicitud
            await _next(context);
        }
    }
}
