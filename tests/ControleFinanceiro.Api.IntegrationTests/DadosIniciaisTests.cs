using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ControleFinanceiro.Api.IntegrationTests.Common;
using FluentAssertions;

namespace ControleFinanceiro.Api.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public class DadosIniciaisTests(IntegrationTestWebAppFactory factory)
{
    [Fact]
    public async Task Register_Should_CriarCategoriasPadraoComSubcategorias()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);

        var categorias = (await GetAsync(client, "/api/v1/categorias")).EnumerateArray().ToList();

        var moradia = categorias.Single(c => c.GetProperty("nome").GetString() == "Moradia");
        moradia.GetProperty("tipo").GetString().Should().Be("Despesa");
        moradia.GetProperty("icone").GetString().Should().Be("building");
        moradia.GetProperty("subcategorias").EnumerateArray()
            .Select(s => s.GetProperty("nome").GetString())
            .Should().Contain("Aluguel");

        categorias.Should().Contain(c => c.GetProperty("nome").GetString() == "Salário"
            && c.GetProperty("tipo").GetString() == "Receita");
    }

    [Fact]
    public async Task Register_Should_CriarTagsPadrao()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);

        var tags = (await GetAsync(client, "/api/v1/tags")).EnumerateArray()
            .Select(t => t.GetProperty("nome").GetString());

        tags.Should().Contain(["Essencial", "Viagem"]);
    }

    [Fact]
    public async Task CategoriaPadrao_Should_SerEditavelERemovivel()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var pets = (await GetAsync(client, "/api/v1/categorias")).EnumerateArray()
            .Single(c => c.GetProperty("nome").GetString() == "Pets");
        var petsId = pets.GetProperty("id").GetGuid();

        var updated = await client.PutAsJsonAsync($"/api/v1/categorias/{petsId}", new { nome = "Cachorro", icone = "paw" });
        updated.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deleted = await client.DeleteAsync($"/api/v1/categorias/{petsId}");
        deleted.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DadosPadrao_Should_PertencerSoAoUsuarioQueSeCadastrou()
    {
        var primeiro = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var segundo = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);

        var idsPrimeiro = (await GetAsync(primeiro, "/api/v1/categorias")).EnumerateArray().Select(c => c.GetProperty("id").GetGuid());
        var idsSegundo = (await GetAsync(segundo, "/api/v1/categorias")).EnumerateArray().Select(c => c.GetProperty("id").GetGuid());

        idsPrimeiro.Should().NotIntersectWith(idsSegundo);
    }

    private static async Task<JsonElement> GetAsync(HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }
}
