using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Contas;

public sealed class CartaoCredito : Entity
{
    public Guid UserId { get; private set; }
    public string Nome { get; private set; }
    public decimal Limite { get; private set; }
    public int DiaFechamento { get; private set; }
    public int DiaVencimento { get; private set; }
    public Guid? ContaPagamentoId { get; private set; }
    public string? Icone { get; private set; }
    public bool Ativo { get; private set; }

    private CartaoCredito(
        Guid id,
        Guid userId,
        string nome,
        decimal limite,
        int diaFechamento,
        int diaVencimento,
        Guid? contaPagamentoId,
        string? icone)
        : base(id)
    {
        UserId = userId;
        Nome = nome;
        Limite = limite;
        DiaFechamento = diaFechamento;
        DiaVencimento = diaVencimento;
        ContaPagamentoId = contaPagamentoId;
        Icone = icone;
        Ativo = true;
    }

    public static Result<CartaoCredito> Create(
        Guid userId,
        string nome,
        decimal limite,
        int diaFechamento,
        int diaVencimento,
        Guid? contaPagamentoId,
        string? icone = null)
    {
        var validacao = Validar(nome, limite, diaFechamento, diaVencimento);
        if (validacao.IsFailure)
            return Result.Failure<CartaoCredito>(validacao.Error);

        return Result.Success(new CartaoCredito(
            Guid.NewGuid(), userId, nome.Trim(), limite, diaFechamento, diaVencimento, contaPagamentoId, Icones.Normalizar(icone)));
    }

    public Result Atualizar(
        string nome,
        decimal limite,
        int diaFechamento,
        int diaVencimento,
        Guid? contaPagamentoId,
        string? icone = null)
    {
        var validacao = Validar(nome, limite, diaFechamento, diaVencimento);
        if (validacao.IsFailure)
            return validacao;

        Nome = nome.Trim();
        Limite = limite;
        DiaFechamento = diaFechamento;
        DiaVencimento = diaVencimento;
        ContaPagamentoId = contaPagamentoId;
        Icone = Icones.Normalizar(icone);
        return Result.Success();
    }

    private static Result Validar(string nome, decimal limite, int diaFechamento, int diaVencimento)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure(CartaoCreditoErrors.NomeObrigatorio);

        if (limite <= 0)
            return Result.Failure(CartaoCreditoErrors.LimiteInvalido);

        if (diaFechamento is < 1 or > 31)
            return Result.Failure(CartaoCreditoErrors.DiaFechamentoInvalido);

        if (diaVencimento is < 1 or > 31)
            return Result.Failure(CartaoCreditoErrors.DiaVencimentoInvalido);

        return Result.Success();
    }

    public void Desativar() => Ativo = false;

    public void Reativar() => Ativo = true;
}
