namespace Chamados.Domain;

/// <summary>
/// Status possíveis do ciclo de vida do chamado.
/// </summary>
public enum StatusChamado
{
    Aberto = 1,
    EmAtendimento = 2,
    Resolvido = 3,
    Cancelado = 4
}