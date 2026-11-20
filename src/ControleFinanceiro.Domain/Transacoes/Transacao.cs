using ControleFinanceiro.Domain.Common;

namespace ControleFinanceiro.Domain.Transacoes;

public sealed class Transacao : Entity
{
    public Guid UserId { get; private set; }
    public TipoTransacao Tipo { get; private set; }
    public decimal Valor { get; private set; }
    public DateOnly Data { get; private set; }
    public string Descricao { get; private set; }
    public Guid? ContaId { get; private set; }
    public Guid? CartaoCreditoId { get; private set; }
    public Guid? ContaDestinoId { get; private set; }
    public Guid? CategoriaId { get; private set; }
    public Guid? SubcategoriaId { get; private set; }
    public StatusTransacao Status { get; private set; }
    public DateOnly? DataPagamento { get; private set; }
    public Guid? RecorrenciaId { get; private set; }

    private readonly List<Guid> _tagIds = [];
    public IReadOnlyList<Guid> TagIds => _tagIds.AsReadOnly();

    private Transacao(
        Guid id,
        Guid userId,
        TipoTransacao tipo,
        decimal valor,
        DateOnly data,
        string descricao,
        Guid? contaId,
        Guid? cartaoCreditoId,
        Guid? contaDestinoId,
        Guid? categoriaId,
        Guid? subcategoriaId,
        StatusTransacao status,
        Guid? recorrenciaId)
        : base(id)
    {
        UserId = userId;
        Tipo = tipo;
        Valor = valor;
        Data = data;
        Descricao = descricao;
        ContaId = contaId;
        CartaoCreditoId = cartaoCreditoId;
        ContaDestinoId = contaDestinoId;
        CategoriaId = categoriaId;
        SubcategoriaId = subcategoriaId;
        Status = status;
        RecorrenciaId = recorrenciaId;
    }

    public static Result<Transacao> Create(
        Guid userId,
        TipoTransacao tipo,
        decimal valor,
        DateOnly data,
        string descricao,
        Guid? contaId,
        Guid? cartaoCreditoId,
        Guid? contaDestinoId,
        Guid? categoriaId,
        Guid? subcategoriaId,
        StatusTransacao status,
        Guid? recorrenciaId)
    {
        if (valor <= 0)
            return Result.Failure<Transacao>(TransacaoErrors.ValorInvalido);

        if (string.IsNullOrWhiteSpace(descricao))
            return Result.Failure<Transacao>(TransacaoErrors.DescricaoObrigatoria);

        var origemValidation = ValidarOrigem(tipo, contaId, cartaoCreditoId, contaDestinoId);
        if (origemValidation.IsFailure)
            return Result.Failure<Transacao>(origemValidation.Error);

        return Result.Success(new Transacao(
            Guid.NewGuid(), userId, tipo, valor, data, descricao.Trim(),
            contaId, cartaoCreditoId, contaDestinoId, categoriaId, subcategoriaId, status, recorrenciaId));
    }

    private static Result ValidarOrigem(TipoTransacao tipo, Guid? contaId, Guid? cartaoCreditoId, Guid? contaDestinoId)
    {
        var temConta = contaId is not null;
        var temCartao = cartaoCreditoId is not null;

        if (temConta == temCartao)
            return Result.Failure(TransacaoErrors.OrigemInvalida);

        if (tipo == TipoTransacao.Transferencia)
        {
            if (temCartao)
                return Result.Failure(TransacaoErrors.TransferenciaNaoPermiteCartao);

            if (contaDestinoId is null)
                return Result.Failure(TransacaoErrors.ContaDestinoObrigatoria);

            if (contaDestinoId == contaId)
                return Result.Failure(TransacaoErrors.ContaDestinoIgualOrigem);
        }
        else if (contaDestinoId is not null)
        {
            return Result.Failure(TransacaoErrors.ContaDestinoNaoPermitida);
        }

        return Result.Success();
    }

    public Result AtualizarDetalhes(
        decimal valor, DateOnly data, string descricao, Guid? categoriaId, Guid? subcategoriaId)
    {
        if (valor <= 0)
            return Result.Failure(TransacaoErrors.ValorInvalido);

        if (string.IsNullOrWhiteSpace(descricao))
            return Result.Failure(TransacaoErrors.DescricaoObrigatoria);

        Valor = valor;
        Data = data;
        Descricao = descricao.Trim();
        CategoriaId = categoriaId;
        SubcategoriaId = subcategoriaId;

        return Result.Success();
    }

    /// <summary>Marca despesa como paga ou receita como recebida, conforme o tipo.</summary>
    public Result MarcarComoEfetivada(DateOnly? dataPagamento = null)
    {
        if (Status.IsExcluido())
            return Result.Failure(TransacaoErrors.StatusInvalido);

        Status = Tipo == TipoTransacao.Receita
            ? StatusTransacao.Recebido
            : StatusTransacao.Pago;

        DataPagamento = dataPagamento ?? Data;

        return Result.Success();
    }

    public void MarcarComoPendente()
    {
        Status = StatusTransacao.Pendente;
        DataPagamento = null;
    }

    public Result Cancelar()
    {
        if (Status.IsRealizado())
            return Result.Failure(TransacaoErrors.NaoPodeCancelarEfetivado);

        Status = StatusTransacao.Cancelado;
        return Result.Success();
    }

    public void AdicionarTag(Guid tagId)
    {
        if (!_tagIds.Contains(tagId))
            _tagIds.Add(tagId);
    }

    public void RemoverTag(Guid tagId) => _tagIds.Remove(tagId);
}
