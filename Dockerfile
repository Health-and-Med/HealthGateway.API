# Etapa 1: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar o arquivo de projeto e restaurar dependências
COPY *.csproj ./
RUN dotnet restore

# Copiar todo o código-fonte para dentro do container
COPY . ./
RUN dotnet publish -c Release -o /publish

# Etapa 2: Runtime otimizado para execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar os arquivos compilados da etapa de build para a execução
COPY --from=build /publish .

# Definir ambiente como Development para exibir Swagger
ENV ASPNETCORE_ENVIRONMENT=Development

# Expor a porta da API Gateway
EXPOSE 80

# Comando para iniciar a API Gateway
ENTRYPOINT ["dotnet", "HealthGateway.API.dll"]
