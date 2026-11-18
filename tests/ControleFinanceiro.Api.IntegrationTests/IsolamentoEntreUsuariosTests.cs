using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ControleFinanceiro.Api.IntegrationTests.Common;
using FluentAssertions;

namespace ControleFinanceiro.Api.IntegrationTests;

/// <summary>
/// Um usuário não pode ler, alterar ou referenciar registros de outro. Para não revelar que o id existe,
/// a API responde 404 como se o registro não existisse.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class IsolamentoEntreUsuariosTests(IntegrationTestWebAppFactory factory)
{
    [Fact]
    public async Task OutroUsuario_Should_ReceberNotFound_When_AcessaContaETransacaoAlheias()
    {
        var dono = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var intruso = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);

        var contaId = await PostForIdAsync(dono, "/api/v1/contas", new { nome = "Conta do dono", tipo = "Corrente", saldoInicial = 100m });
        var transacaoId = await PostForIdAsync(dono, "/api/v1/transacoes", new
        {
            tipo = "Despesa",
            valor = 40m,
            data = "2026-10-05",
            descricao = "Mercado",
            contaId,
            status = "Pendente",
        });

        (await intruso.GetAsync($"/api/v1/contas/{contaId}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await intruso.PostAsync($"/api/v1/contas/{contaId}/desativar", null)).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await intruso.GetAsync($"/api/v1/transacoes/{transacaoId}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await intruso.PostAsync($"/api/v1/transacoes/{transacaoId}/marcar-pago", null)).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await intruso.PutAsJsonAsync($"/api/v1/transacoes/{transacaoId}", new
        {
            valor = 1m,
            data = "2026-10-05",
            descricao = "Alterada pelo intruso",
        })).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await intruso.DeleteAsync($"/api/v1/transacoes/{transacaoId}")).StatusCode.Should().Be(HttpStatusCode.NotFound);

        var transacao = await (await dono.GetAsync($"/api/v1/transacoes/{transacaoId}")).Content.ReadFromJsonAsync<JsonElement>();
        transacao.GetProperty("descricao").GetString().Should().Be("Mercado");
        transacao.GetProperty("status").GetString().Should().Be("Pendente");

        var conta = await (await dono.GetAsync($"/api/v1/contas/{contaId}")).Content.ReadFromJsonAsync<JsonElement>();
        conta.GetProperty("ativo").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task CreateTransacao_Should_ReturnNotFound_When_ContaPertenceAOutroUsuario()
    {
        var dono = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var intruso = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var contaDoDono = await PostForIdAsync(dono, "/api/v1/contas", new { nome = "Conta do dono", tipo = "Corrente", saldoInicial = 0m });

        var response = await intruso.PostAsJsonAsync("/api/v1/transacoes", new
        {
            tipo = "Receita",
            valor = 1000m,
            data = "2026-10-05",
            descricao = "Lançamento na conta alheia",
            contaId = contaDoDono,
            status = "Pago",
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteObjetivo_Should_ReturnNotFound_When_ObjetivoPertenceAOutroUsuario()
    {
        var dono = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var intruso = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var objetivoId = await PostForIdAsync(dono, "/api/v1/objetivos", new { nome = "Reserva", valorAlvo = 1000m, dataAlvo = "2027-12-31" });

        (await intruso.DeleteAsync($"/api/v1/objetivos/{objetivoId}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await intruso.PostAsJsonAsync($"/api/v1/objetivos/{objetivoId}/aportes", new { valor = 10m, data = "2026-10-05" }))
            .StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await dono.GetAsync($"/api/v1/objetivos/{objetivoId}")).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static async Task<Guid> PostForIdAsync(HttpClient client, string url, object body)
    {
        var response = await client.PostAsJsonAsync(url, body);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("id").GetGuid();
    }
}
