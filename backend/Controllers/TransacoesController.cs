using GastosResidenciais.API.Application.Services;
using GastosResidenciais.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TransacoesController(ITransacaoService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TransacaoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar() =>
        Ok(await service.ListarAsync());

    [HttpPost]
    [ProducesResponseType(typeof(TransacaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Criar([FromBody] TransacaoRequest dto)
    {
        var resultado = await service.CriarAsync(dto);

        if (resultado.Erro is not null)
            return UnprocessableEntity(new { erro = resultado.Erro });

        return StatusCode(201, resultado.Data);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var removida = await service.DeletarAsync(id);
        return removida ? NoContent() : NotFound();
    }
}
