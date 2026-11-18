using ControleFinanceiro.Domain.Orcamentos;

namespace ControleFinanceiro.Application.Orcamentos;

internal static class OrcamentoMapper
{
    public static OrcamentoResponse ToResponse(Orcamento orcamento, ConsumoOrcamento consumo)
    {
        var valorConsumido = consumo.Total;
        var percentual = orcamento.LimiteValor == 0 ? 0 : valorConsumido / orcamento.LimiteValor * 100m;

        return new OrcamentoResponse
        {
            Id = orcamento.Id,
            CategoriaId = orcamento.CategoriaId,
            Ano = orcamento.Ano,
            Mes = orcamento.Mes,
            LimiteValor = orcamento.LimiteValor,
            ValorConsumido = valorConsumido,
            ValorPendente = consumo.Pendente,
            PercentualConsumido = Math.Round(percentual, 2),
            Estourado = valorConsumido > orcamento.LimiteValor,
        };
    }
}
