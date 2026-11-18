using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ControleFinanceiro.Api.IntegrationTests.Common;
using FluentAssertions;

namespace ControleFinanceiro.Api.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public class LimiteOrcamentoEEdicaoTests(IntegrationTestWebAppFactory factory)
{
    [Fact]
    public async Task LimiteDisponivel_Should_SerLiberado_When_DespesaNoCartaoEhPaga()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var cartaoId = await PostForIdAsync(client, "/api/v1/cartoes-credito", new { nome = "Cartão", limite = 1000m, diaFechamento = 1, diaVencimento = 10 });
        var transacaoId = await PostForIdAsync(client, "/api/v1/transacoes", new
        {
            tipo = "Despesa",
            valor = 300m,
            data = "2026-10-06",
            descricao = "Notebook",
            cartaoCreditoId = cartaoId,
            status = "Pendente",
        });

        (await GetJsonAsync(client, $"/api/v1/cartoes-credito/{cartaoId}")).GetProperty("limiteDisponivel").GetDecimal().Should().Be(700m);

        await client.PostAsync($"/api/v1/transacoes/{transacaoId}/marcar-pago", null);

        (await GetJsonAsync(client, $"/api/v1/cartoes-credito/{cartaoId}")).GetProperty("limiteDisponivel").GetDecimal().Should().Be(1000m);
    }

    [Fact]
    public async Task GetOrcamentos_Should_SepararValorPendente_When_HaDespesasPagasEPendentes()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var categoriaId = await PostForIdAsync(client, "/api/v1/categorias", new { nome = "Lazer", tipo = "Despesa" });
        var contaId = await PostForIdAsync(client, "/api/v1/contas", new { nome = "Conta", tipo = "Corrente", saldoInicial = 0m });
        await PostForIdAsync(client, "/api/v1/orcamentos", new { categoriaId, ano = 2026, mes = 10, limiteValor = 200m });

        foreach (var (valor, status) in new[] { (100m, "Pago"), (50m, "Pendente") })
        {
            await PostForIdAsync(client, "/api/v1/transacoes", new
            {
                tipo = "Despesa",
                valor,
                data = "2026-10-06",
                descricao = "Cinema",
                contaId,
                categoriaId,
                status,
            });
        }

        var orcamentos = await GetJsonAsync(client, "/api/v1/orcamentos?ano=2026&mes=10");
        var orcamento = orcamentos.EnumerateArray().Single(o => o.GetProperty("categoriaId").GetGuid() == categoriaId);

        orcamento.GetProperty("valorConsumido").GetDecimal().Should().Be(150m);
        orcamento.GetProperty("valorPendente").GetDecimal().Should().Be(50m);
    }

    [Fact]
    public async Task UpdateCartaoCredito_Should_AlterarDados_When_UsuarioEhDono()
    {
        var client = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var cartaoId = await PostForIdAsync(client, "/api/v1/cartoes-credito", new { nome = "Cartão", limite = 1000m, diaFechamento = 1, diaVencimento = 10 });

        var response = await client.PutAsJsonAsync($"/api/v1/cartoes-credito/{cartaoId}", new { nome = "Cartão Gold", limite = 3000m, diaFechamento = 25, diaVencimento = 5 });

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var cartao = await GetJsonAsync(client, $"/api/v1/cartoes-credito/{cartaoId}");
        cartao.GetProperty("nome").GetString().Should().Be("Cartão Gold");
        cartao.GetProperty("limite").GetDecimal().Should().Be(3000m);
        cartao.GetProperty("diaFechamento").GetInt32().Should().Be(25);
    }

    [Fact]
    public async Task UpdateObjetivo_Should_AlterarDados_When_UsuarioEhDono_E_RetornarNotFound_ParaOutroUsuario()
    {
        var dono = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var intruso = await AuthTestHelper.CreateAuthenticatedClientAsync(factory);
        var objetivoId = await PostForIdAsync(dono, "/api/v1/objetivos", new { nome = "Viagem", valorAlvo = 5000m, dataAlvo = "2027-01-31" });
        var alteracao = new { nome = "Viagem para Floripa", valorAlvo = 6000m, dataAlvo = "2027-07-15" };

        (await intruso.PutAsJsonAsync($"/api/v1/objetivos/{objetivoId}", alteracao)).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await dono.PutAsJsonAsync($"/api/v1/objetivos/{objetivoId}", alteracao)).StatusCode.Should().Be(HttpStatusCode.NoContent);

        var objetivo = await GetJsonAsync(dono, $"/api/v1/objetivos/{objetivoId}");
        objetivo.GetProperty("nome").GetString().Should().Be("Viagem para Floripa");
        objetivo.GetProperty("valorAlvo").GetDecimal().Should().Be(6000m);
    }

    private static async Task<JsonElement> GetJsonAsync(HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }

    private static async Task<Guid> PostForIdAsync(HttpClient client, string url, object body)
    {
        var response = await client.PostAsJsonAsync(url, body);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("id").GetGuid();
    }
}
