using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Prometheus;

public class OcelotAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<OcelotAuthMiddleware> _logger;

    // 🔹 Definição de métricas Prometheus
    private static readonly Counter RequestCounter = Metrics
        .CreateCounter("gateway_http_requests_total", "Total de requisições HTTP recebidas pelo API Gateway",
            new CounterConfiguration
            {
                LabelNames = new[] { "status_code", "method", "path" }
            });

    private static readonly Histogram RequestDuration = Metrics
        .CreateHistogram("gateway_request_duration_seconds", "Duração das requisições do API Gateway",
            new HistogramConfiguration
            {
                Buckets = Histogram.LinearBuckets(start: 0.1, width: 0.1, count: 10),
                LabelNames = new[] { "method", "path" }
            });

    public OcelotAuthMiddleware(RequestDelegate next, ILogger<OcelotAuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.ToString();
        var method = context.Request.Method;

        _logger.LogInformation($"Interceptando requisição para {path}");

        // 🔹 Criar um temporizador para medir o tempo da requisição
        var timer = RequestDuration.WithLabels(method, path).NewTimer();

        try
        {
            await _next(context);
        }
        finally
        {
            // 🔹 Registrar a métrica do Prometheus com o código de status da resposta
            var statusCode = context.Response.StatusCode.ToString();
            RequestCounter.WithLabels(statusCode, method, path).Inc();

            // 🔹 Finalizar temporizador e registrar tempo da requisição
            timer.Dispose();
        }
    }
}

