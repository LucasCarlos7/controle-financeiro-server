using ControleFinanceiro.Domain.Objetivos;

namespace ControleFinanceiro.Application.Objetivos;

internal static class ObjetivoMapper
{
    public static ObjetivoResponse ToResponse(Objetivo objetivo, IReadOnlyList<Aporte> aportes)
    {
        var valorAtual = aportes.Sum(a => a.Valor);
        var percentual = objetivo.ValorAlvo == 0 ? 0 : valorAtual / objetivo.ValorAlvo * 100m;

        return new ObjetivoResponse
        {
            Id = objetivo.Id,
            Nome = objetivo.Nome,
            ValorAlvo = objetivo.ValorAlvo,
            ValorAtual = valorAtual,
            PercentualConcluido = Math.Round(Math.Min(percentual, 100m), 2),
            DataAlvo = objetivo.DataAlvo,
            Aportes = aportes
                .Select(a => new AporteResponse { Id = a.Id, Valor = a.Valor, Data = a.Data })
                .ToList(),
        };
    }
}
