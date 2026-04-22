using GastosResidenciais.API.Application.Services;
using GastosResidenciais.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GastosResidenciais.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RelatoriosController(IRelatorioService service) : ControllerBase
{
    [HttpGet("por-pessoa")]
    [ProducesResponseType(typeof(RelatorioPorPessoaResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> TotaisPorPessoa() =>
        Ok(await service.TotaisPorPessoaAsync());

    [HttpGet("por-categoria")]
    [ProducesResponseType(typeof(RelatorioPorCategoriaResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> TotaisPorCategoria() =>
        Ok(await service.TotaisPorCategoriaAsync());
}
