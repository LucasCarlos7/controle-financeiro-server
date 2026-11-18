FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Directory.Build.props ./
COPY ControleFinanceiro.sln ./
COPY src/ControleFinanceiro.Domain/ControleFinanceiro.Domain.csproj src/ControleFinanceiro.Domain/
COPY src/ControleFinanceiro.Application/ControleFinanceiro.Application.csproj src/ControleFinanceiro.Application/
COPY src/ControleFinanceiro.Infrastructure/ControleFinanceiro.Infrastructure.csproj src/ControleFinanceiro.Infrastructure/
COPY src/ControleFinanceiro.Api/ControleFinanceiro.Api.csproj src/ControleFinanceiro.Api/
RUN dotnet restore src/ControleFinanceiro.Api/ControleFinanceiro.Api.csproj

COPY src/ src/
RUN dotnet publish src/ControleFinanceiro.Api/ControleFinanceiro.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ControleFinanceiro.Api.dll"]
