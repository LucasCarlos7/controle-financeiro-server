using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ControleFinanceiro.Api.IntegrationTests.Common;

internal sealed record AuthResponseDto(Guid UserId, string Nome, string Email, string Token);

internal static class AuthTestHelper
{
    public static async Task<AuthResponseDto> RegisterAsync(
        HttpClient client, string? email = null, string senha = "SenhaForte123")
    {
        var request = new
        {
            nome = "Usuário de Teste",
            email = email ?? $"{Guid.NewGuid()}@example.com",
            senha,
        };

        var response = await client.PostAsJsonAsync("/api/v1/Auth/registrar", request);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return body ?? throw new InvalidOperationException("Resposta de registro vazia.");
    }

    public static async Task<HttpClient> CreateAuthenticatedClientAsync(IntegrationTestWebAppFactory factory)
    {
        var client = factory.CreateClient();
        var auth = await RegisterAsync(client);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);
        return client;
    }
}
