using Chamados.Domain;

namespace Chamados.Domain.Tests;

public class ChamadoTests
{
    [Fact]
    public void NovoChamado_DeveNascerComStatusAberto()
    {
        var chamado = new Chamado("Corrigir vazamento");
        Assert.Equal(StatusChamado.Aberto, chamado.Status);
    }

    [Fact]
    public void CriarChamado_ComTituloEmBranco_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() => new Chamado(""));
    }
}