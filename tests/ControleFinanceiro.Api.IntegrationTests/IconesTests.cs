using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ControleFinanceiro.Api.IntegrationTests.Common;
using FluentAssertions;

namespace ControleFinanceiro.Api.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public class IconesTests(IntegrationTestWebAppFactory factory)
{
    [Fact]
    public async Task Conta_Should_GuardarEAtualizarIcone()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);

        var created = await client.PostAsJsonAsync("/api/v1/contas", new { nome = "Nubank", tipo = "Corrente", icone = "bank" });
        var contaId = await ReadIdAsync(created);

        (await GetAsync(client, $"/api/v1/contas/{contaId}")).GetProperty("icone").GetString().Should().Be("bank");

        var updated = await client.PutAsJsonAsync($"/api/v1/contas/{contaId}", new { nome = "Nubank", icone = "wallet" });
        updated.StatusCode.Should().Be(HttpStatusCode.NoContent);

        (await GetAsync(client, $"/api/v1/contas/{contaId}")).GetProperty("icone").GetString().Should().Be("wallet");
    }

    [Fact]
    public async Task CartaoCredito_Should_GuardarIcone()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);

        var created = await client.PostAsJsonAsync("/api/v1/cartoes-credito", new
        {
            nome = "Nubank",
            limite = 5000m,
            diaFechamento = 3,
            diaVencimento = 10,
            icone = "card",
        });
        var cartaoId = await ReadIdAsync(created);

        (await GetAsync(client, $"/api/v1/cartoes-credito/{cartaoId}")).GetProperty("icone").GetString().Should().Be("card");
    }

    [Fact]
    public async Task Tag_Should_AtualizarNomeEIcone()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);

        var tagId = await ReadIdAsync(await client.PostAsJsonAsync("/api/v1/tags", new { nome = "viagem", icone = "plane" }));

        var updated = await client.PutAsJsonAsync($"/api/v1/tags/{tagId}", new { nome = "férias", icone = "sun" });
        updated.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var tags = await GetAsync(client, "/api/v1/tags");
        var tag = tags.EnumerateArray().Single(t => t.GetProperty("id").GetGuid() == tagId);
        tag.GetProperty("nome").GetString().Should().Be("férias");
        tag.GetProperty("icone").GetString().Should().Be("sun");
    }

    [Fact]
    public async Task UpdateTag_Should_ReturnConflict_When_NomeJaUsadoPorOutraTag()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);

        await client.PostAsJsonAsync("/api/v1/tags", new { nome = "trabalho" });
        var tagId = await ReadIdAsync(await client.PostAsJsonAsync("/api/v1/tags", new { nome = "pessoal" }));

        var response = await client.PutAsJsonAsync($"/api/v1/tags/{tagId}", new { nome = "trabalho" });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateTag_Should_ReturnNotFound_When_TagNaoExiste()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);

        var response = await client.PutAsJsonAsync($"/api/v1/tags/{Guid.NewGuid()}", new { nome = "qualquer" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task<Guid> ReadIdAsync(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("id").GetGuid();
    }

    private static async Task<JsonElement> GetAsync(HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }
}
