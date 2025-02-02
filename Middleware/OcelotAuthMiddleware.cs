using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class OcelotAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<OcelotAuthMiddleware> _logger;

    public OcelotAuthMiddleware(RequestDelegate next, ILogger<OcelotAuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        _logger.LogInformation($"Interceptando requisição para {context.Request.Path}");

        // 🔹 Apenas loga a requisição e deixa o Ocelot lidar com a autenticação
        await _next(context);
    }
}
