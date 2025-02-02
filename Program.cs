using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Carregar configuração do Ocelot
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddOcelot(); // 🔹 Removemos a autenticação JWT do Ocelot

// 🔹 Adicionar suporte ao Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.AddConsole();

var app = builder.Build();

// 🔹 Middleware de métricas (Prometheus)
app.UseMetricServer();
app.UseHttpMetrics(options =>
{
    options.AddCustomLabel("path", context => context.Request.Path);
});

// 🔹 Middleware para garantir que o token JWT seja propagado corretamente para os serviços downstream
app.Use(async (context, next) =>
{
    if (context.Request.Headers.ContainsKey("Authorization"))
    {
        var token = context.Request.Headers["Authorization"];
        context.Request.Headers.Remove("Authorization");
        context.Request.Headers.Add("Authorization", token);
    }
    await next();
});

// 🔹 Ativa o Ocelot
app.UseOcelot().Wait();

app.Run();
