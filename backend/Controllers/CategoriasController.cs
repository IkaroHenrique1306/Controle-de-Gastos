using GastosResidenciais.API.Application.Services;
using GastosResidenciais.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.API.Controllers;

// Categorias só podem ser criadas e removidas — sem edição, conforme especificação.
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriasController(ICategoriaService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar() =>
        Ok(await service.ListarAsync());

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var categoria = await service.ObterPorIdAsync(id);
        return categoria is null ? NotFound() : Ok(categoria);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CategoriaRequest dto)
    {
        var criada = await service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // 409 Conflict quando há transações vinculadas — remoção bloqueada por integridade referencial.
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Deletar(Guid id)
    {
        var (removida, erro) = await service.DeletarAsync(id);

        if (!removida && erro is not null)
            return Conflict(new { erro });

        return removida ? NoContent() : NotFound();
    }
}
