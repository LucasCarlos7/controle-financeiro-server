using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ControleFinanceiro.Api.IntegrationTests.Common;
using FluentAssertions;

namespace ControleFinanceiro.Api.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public class OrcamentosTests(IntegrationTestWebAppFactory factory)
{
    [Fact]
    public async Task CreateOrcamento_Should_ReturnConflict_When_JaExisteParaCategoriaEMes()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var categoriaId = await CreateCategoriaAsync(client);

        await client.PostAsJsonAsync("/api/v1/orcamentos", new
        {
            categoriaId,
            ano = 2026,
            mes = 8,
            limiteValor = 500m,
        });

        var response = await client.PostAsJsonAsync("/api/v1/orcamentos", new
        {
            categoriaId,
            ano = 2026,
            mes = 8,
            limiteValor = 300m,
        });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetOrcamentos_Should_CalcularPercentualConsumido_When_HaDespesasNaCategoria()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var categoriaId = await CreateCategoriaAsync(client);
        var contaId = await CreateContaAsync(client);

        await client.PostAsJsonAsync("/api/v1/orcamentos", new
        {
            categoriaId,
            ano = 2026,
            mes = 8,
            limiteValor = 500m,
        });

        await client.PostAsJsonAsync("/api/v1/transacoes", new
        {
            tipo = "Despesa",
            valor = 150m,
            data = "2026-08-05",
            descricao = "Compra",
            contaId,
            categoriaId,
            status = "Pago",
        });

        var response = await client.GetAsync("/api/v1/orcamentos?ano=2026&mes=8");
        var orcamentos = await response.Content.ReadFromJsonAsync<JsonElement>();
        var orcamento = orcamentos.EnumerateArray().Single(o => o.GetProperty("categoriaId").GetGuid() == categoriaId);

        orcamento.GetProperty("valorConsumido").GetDecimal().Should().Be(150m);
        orcamento.GetProperty("percentualConsumido").GetDecimal().Should().Be(30.00m);
        orcamento.GetProperty("estourado").GetBoolean().Should().BeFalse();
    }

    private static async Task<Guid> CreateCategoriaAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/v1/categorias", new
        {
            nome = "Alimentacao",
            tipo = "Despesa",
        });

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateContaAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/v1/contas", new
        {
            nome = "Conta Corrente",
            tipo = "Corrente",
            saldoInicial = 1000m,
        });

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("id").GetGuid();
    }
}
