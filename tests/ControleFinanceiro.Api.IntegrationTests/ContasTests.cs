using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ControleFinanceiro.Api.IntegrationTests.Common;
using FluentAssertions;

namespace ControleFinanceiro.Api.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public class ContasTests(IntegrationTestWebAppFactory factory)
{
    [Fact]
    public async Task CreateConta_Should_ReturnCreated_When_DadosValidos()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);

        var response = await client.PostAsJsonAsync("/api/v1/contas", new
        {
            nome = "Conta Corrente",
            tipo = "Corrente",
            saldoInicial = 100m,
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateConta_Should_ReturnBadRequest_When_NomeVazio()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);

        var response = await client.PostAsJsonAsync("/api/v1/contas", new
        {
            nome = "",
            tipo = "Corrente",
            saldoInicial = 0m,
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetContaById_Should_ReturnNotFound_When_ContaNaoExiste()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);

        var response = await client.GetAsync($"/api/v1/contas/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SaldoAtual_Should_RefletirTransacaoPaga_When_DespesaLancada()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);

        var contaId = await CreateContaAsync(client, saldoInicial: 500m);

        await client.PostAsJsonAsync("/api/v1/transacoes", new
        {
            tipo = "Despesa",
            valor = 150.50m,
            data = "2026-08-10",
            descricao = "Supermercado",
            contaId,
            status = "Pago",
        });

        var response = await client.GetAsync($"/api/v1/contas/{contaId}");
        var conta = await response.Content.ReadFromJsonAsync<JsonElement>();

        conta.GetProperty("saldoAtual").GetDecimal().Should().Be(349.50m);
    }

    [Fact]
    public async Task DeactivateConta_Should_MarcarComoInativa()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var contaId = await CreateContaAsync(client);

        var deactivateResponse = await client.PostAsync($"/api/v1/contas/{contaId}/desativar", null);
        deactivateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/api/v1/contas/{contaId}");
        var conta = await getResponse.Content.ReadFromJsonAsync<JsonElement>();
        conta.GetProperty("ativo").GetBoolean().Should().BeFalse();
    }

    private static async Task<Guid> CreateContaAsync(HttpClient client, decimal saldoInicial = 0m)
    {
        var response = await client.PostAsJsonAsync("/api/v1/contas", new
        {
            nome = "Conta Corrente",
            tipo = "Corrente",
            saldoInicial,
        });

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("id").GetGuid();
    }
}
