using System.Net.Http.Json;
using System.Text.Json;
using ControleFinanceiro.Api.IntegrationTests.Common;
using FluentAssertions;

namespace ControleFinanceiro.Api.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public class ObjetivosTests(IntegrationTestWebAppFactory factory)
{
    [Fact]
    public async Task GetObjetivos_Should_CalcularProgresso_When_HaAportes()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);

        var createResponse = await client.PostAsJsonAsync("/api/v1/objetivos", new
        {
            nome = "Viagem",
            valorAlvo = 5000m,
            dataAlvo = "2026-12-31",
        });
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var objetivoId = created.GetProperty("id").GetGuid();

        await client.PostAsJsonAsync($"/api/v1/objetivos/{objetivoId}/aportes", new
        {
            valor = 1000m,
            data = "2026-08-10",
        });

        var response = await client.GetAsync($"/api/v1/objetivos/{objetivoId}");
        var objetivo = await response.Content.ReadFromJsonAsync<JsonElement>();

        objetivo.GetProperty("valorAtual").GetDecimal().Should().Be(1000m);
        objetivo.GetProperty("percentualConcluido").GetDecimal().Should().Be(20.00m);
        objetivo.GetProperty("aportes").GetArrayLength().Should().Be(1);
    }

    [Fact]
    public async Task CreateAporte_Should_ReturnNotFound_When_ObjetivoNaoExiste()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);

        var response = await client.PostAsJsonAsync($"/api/v1/objetivos/{Guid.NewGuid()}/aportes", new
        {
            valor = 100m,
            data = "2026-08-10",
        });

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
