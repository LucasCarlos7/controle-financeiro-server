using ControleFinanceiro.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace ControleFinanceiro.Api.IntegrationTests;

public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("controle_financeiro_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Fora de Development os User Secrets da máquina não são carregados: os testes nunca herdam o banco
        // ou o segredo JWT de quem os executa. UseSetting já vale para as leituras feitas no Program antes do Build.
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DefaultConnection", _dbContainer.GetConnectionString());
        builder.UseSetting("Jwt:Secret", "integration-tests-secret-key-not-for-production-use-only-1234567890");
        builder.UseSetting("Jwt:Issuer", "ControleFinanceiro.Api.Tests");
        builder.UseSetting("Jwt:Audience", "ControleFinanceiro.Client.Tests");
        builder.UseSetting("Jwt:ExpirationMinutes", "60");
        // Cada teste registra ao menos um usuário; o limite de produção (10/min) derrubaria a suíte.
        builder.UseSetting("RateLimiting:AuthPermitLimit", "1000");

        // Garantia extra: o DbContext aponta para o container mesmo que a connection string seja lida de outra fonte.
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(_dbContainer.GetConnectionString()));
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
