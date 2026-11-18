using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ControleFinanceiro.Api.IntegrationTests.Common;
using FluentAssertions;

namespace ControleFinanceiro.Api.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public class ContaECartaoInativosTests(IntegrationTestWebAppFactory factory)
{
    [Fact]
    public async Task CreateTransacao_Should_ReturnUnprocessable_When_ContaInativa()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var contaId = await PostForIdAsync(client, "/api/v1/contas", new { nome = "Antiga", tipo = "Corrente", saldoInicial = 0m });
        await client.PostAsync($"/api/v1/contas/{contaId}/desativar", null);

        var response = await client.PostAsJsonAsync("/api/v1/transacoes", Despesa(contaId: contaId));

        await AssertErroAsync(response, "Conta.Inativa");
    }

    [Fact]
    public async Task CreateTransacao_Should_ReturnUnprocessable_When_ContaDestinoInativa()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var origem = await PostForIdAsync(client, "/api/v1/contas", new { nome = "Origem", tipo = "Corrente", saldoInicial = 100m });
        var destino = await PostForIdAsync(client, "/api/v1/contas", new { nome = "Destino", tipo = "Poupanca", saldoInicial = 0m });
        await client.PostAsync($"/api/v1/contas/{destino}/desativar", null);

        var response = await client.PostAsJsonAsync("/api/v1/transacoes", new
        {
            tipo = "Transferencia",
            valor = 50m,
            data = "2026-10-06",
            descricao = "Reserva",
            contaId = origem,
            contaDestinoId = destino,
            status = "Pago",
        });

        await AssertErroAsync(response, "Conta.Inativa");
    }

    [Fact]
    public async Task CreateTransacao_Should_ReturnUnprocessable_When_CartaoInativo()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var cartaoId = await PostForIdAsync(client, "/api/v1/cartoes-credito", new { nome = "Cartão", limite = 1000m, diaFechamento = 1, diaVencimento = 10 });
        await client.PostAsync($"/api/v1/cartoes-credito/{cartaoId}/desativar", null);

        var response = await client.PostAsJsonAsync("/api/v1/transacoes", Despesa(cartaoCreditoId: cartaoId));

        await AssertErroAsync(response, "CartaoCredito.Inativo");
    }

    [Fact]
    public async Task MarcarPago_Should_Funcionar_When_TransacaoExistenteEmContaDesativada()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var contaId = await PostForIdAsync(client, "/api/v1/contas", new { nome = "Encerrando", tipo = "Corrente", saldoInicial = 0m });
        var transacaoId = await PostForIdAsync(client, "/api/v1/transacoes", Despesa(contaId: contaId, status: "Pendente"));
        await client.PostAsync($"/api/v1/contas/{contaId}/desativar", null);

        var response = await client.PostAsync($"/api/v1/transacoes/{transacaoId}/marcar-pago", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private static object Despesa(Guid? contaId = null, Guid? cartaoCreditoId = null, string status = "Pago") => new
    {
        tipo = "Despesa",
        valor = 30m,
        data = "2026-10-06",
        descricao = "Compra",
        contaId,
        cartaoCreditoId,
        status,
    };

    private static async Task AssertErroAsync(HttpResponseMessage response, string codigo)
    {
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        problem.GetProperty("title").GetString().Should().Be(codigo);
    }

    private static async Task<Guid> PostForIdAsync(HttpClient client, string url, object body)
    {
        var response = await client.PostAsJsonAsync(url, body);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("id").GetGuid();
    }
}
