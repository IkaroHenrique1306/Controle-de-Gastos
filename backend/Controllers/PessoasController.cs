using GastosResidenciais.API.Application.Services;
using GastosResidenciais.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.API.Controllers;

// Controller sem lógica de negócio — delega tudo ao IPessoaService.
// [ApiController] habilita validação automática de ModelState e binding por convenção.
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PessoasController(IPessoaService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PessoaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar() =>
        Ok(await service.ListarAsync());

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PessoaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var pessoa = await service.ObterPorIdAsync(id);
        return pessoa is null ? NotFound() : Ok(pessoa);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PessoaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] PessoaRequest dto)
    {
        var criada = await service.CriarAsync(dto);
        // CreatedAtAction retorna o header Location apontando para o endpoint de busca por id.
        return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PessoaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] PessoaRequest dto)
    {
        var atualizada = await service.AtualizarAsync(id, dto);
        return atualizada is null ? NotFound() : Ok(atualizada);
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
