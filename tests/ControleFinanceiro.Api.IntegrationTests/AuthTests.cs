using System.Net;
using System.Net.Http.Json;
using ControleFinanceiro.Api.IntegrationTests.Common;
using FluentAssertions;

namespace ControleFinanceiro.Api.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public class AuthTests(IntegrationTestWebAppFactory factory)
{
    [Fact]
    public async Task Register_Should_ReturnToken_When_DadosValidos()
    {
        var client = factory.CreateClient();

        var auth = await AuthTestHelper.RegisterAsync(client);

        auth.Token.Should().NotBeNullOrWhiteSpace();
        auth.UserId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Register_Should_ReturnConflict_When_EmailJaCadastrado()
    {
        var client = factory.CreateClient();
        var email = $"{Guid.NewGuid()}@example.com";
        await AuthTestHelper.RegisterAsync(client, email);

        var response = await client.PostAsJsonAsync("/api/v1/Auth/registrar", new
        {
            nome = "Outro Usuário",
            email,
            senha = "OutraSenhaForte123",
        });

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_Should_ReturnToken_When_CredenciaisValidas()
    {
        var client = factory.CreateClient();
        var email = $"{Guid.NewGuid()}@example.com";
        const string senha = "SenhaForte123";
        await AuthTestHelper.RegisterAsync(client, email, senha);

        var response = await client.PostAsJsonAsync("/api/v1/Auth/entrar", new { email, senha });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        body!.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_Should_ReturnUnauthorized_When_SenhaIncorreta()
    {
        var client = factory.CreateClient();
        var email = $"{Guid.NewGuid()}@example.com";
        await AuthTestHelper.RegisterAsync(client, email, "SenhaForte123");

        var response = await client.PostAsJsonAsync(
            "/api/v1/Auth/entrar", new { email, senha = "SenhaErrada123" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_Should_ReturnUnauthorized_When_SemToken()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/contas");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
