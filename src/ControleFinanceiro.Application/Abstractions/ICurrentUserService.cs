namespace ControleFinanceiro.Application.Abstractions;

public interface ICurrentUserService
{
    Guid UserId { get; }
}
