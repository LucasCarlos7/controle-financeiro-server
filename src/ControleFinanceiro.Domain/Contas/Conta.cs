using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Contas;

public sealed class Conta : Entity
{
    public Guid UserId { get; private set; }
    public string Nome { get; private set; }
    public TipoConta Tipo { get; private set; }
    public decimal SaldoInicial { get; private set; }
    public string? Icone { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime CriadoEm { get; private set; }

    private Conta(Guid id, Guid userId, string nome, TipoConta tipo, decimal saldoInicial, string? icone, DateTime criadoEm)
        : base(id)
    {
        UserId = userId;
        Nome = nome;
        Tipo = tipo;
        SaldoInicial = saldoInicial;
        Icone = icone;
        Ativo = true;
        CriadoEm = criadoEm;
    }

    public static Result<Conta> Create(Guid userId, string nome, TipoConta tipo, decimal saldoInicial, string? icone = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure<Conta>(ContaErrors.NomeObrigatorio);

        return Result.Success(new Conta(Guid.NewGuid(), userId, nome.Trim(), tipo, saldoInicial, Icones.Normalizar(icone), DateTime.UtcNow));
    }

    public Result AtualizarDetalhes(string nome, string? icone)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure(ContaErrors.NomeObrigatorio);

        Nome = nome.Trim();
        Icone = Icones.Normalizar(icone);
        return Result.Success();
    }

    public void Desativar() => Ativo = false;

    public void Reativar() => Ativo = true;
}
