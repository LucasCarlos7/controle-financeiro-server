using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ControleFinanceiro.Api.IntegrationTests.Common;
using FluentAssertions;

namespace ControleFinanceiro.Api.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public class TransacoesRecorrentesTests(IntegrationTestWebAppFactory factory)
{
    private const int TotalParcelasPadrao = 100;

    [Theory]
    [InlineData("Mensal")]
    [InlineData("Parcelada")]
    public async Task CreateRecorrente_Should_Gerar100Lancamentos_When_TotalParcelasNaoInformado(string tipoRecorrencia)
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var contaId = await CriarContaAsync(client);

        var response = await client.PostAsJsonAsync("/api/v1/transacoes/recorrente", new
        {
            tipo = "Despesa",
            valor = 50m,
            dataInicio = "2026-01-10",
            descricao = "Assinatura",
            contaId,
            tipoRecorrencia,
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var transacoes = await ListarRecorrentesAsync(client);
        transacoes.Should().HaveCount(TotalParcelasPadrao);
    }

    [Fact]
    public async Task CreateRecorrente_Should_RespeitarTotalParcelas_When_Informado()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var contaId = await CriarContaAsync(client);

        var response = await client.PostAsJsonAsync("/api/v1/transacoes/recorrente", new
        {
            tipo = "Despesa",
            valor = 300m,
            dataInicio = "2026-01-10",
            descricao = "Notebook",
            contaId,
            tipoRecorrencia = "Parcelada",
            totalParcelas = 10,
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var transacoes = await ListarRecorrentesAsync(client);
        transacoes.Should().HaveCount(10);
    }

    private static async Task<Guid> CriarContaAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/v1/contas", new { nome = "Corrente", tipo = "Corrente", saldoInicial = 0m });
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("id").GetGuid();
    }

    private static async Task<JsonElement[]> ListarRecorrentesAsync(HttpClient client)
    {
        var response = await client.GetAsync(
            "/api/v1/transacoes?apenasRecorrentes=true&dataInicio=2026-01-01&dataFim=2040-12-31");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return [.. json.EnumerateArray()];
    }
}
