using Casefile.Api.Models;
using Casefile.Api.Services;
using Casefile.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Casefile.Api.Controllers;

[ApiController]
[Route("api/cases")]
public sealed class CasesController(ICaseService cases) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CaseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CaseResponse>> Create([FromBody] CreateCaseRequest request, CancellationToken ct)
    {
        var created = await cases.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedCasesResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedCasesResponse>> List(
        [FromQuery] CaseStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        return Ok(await cases.ListAsync(status, page, pageSize, ct));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CaseResponse>> GetById(Guid id, CancellationToken ct)
    {
        return Ok(await cases.GetAsync(id, ct));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CaseResponse>> Update(Guid id, [FromBody] UpdateCaseRequest request, CancellationToken ct)
    {
        return Ok(await cases.UpdateAsync(id, request, ct));
    }

    [HttpPost("{id:guid}/status")]
    [ProducesResponseType(typeof(CaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CaseResponse>> ChangeStatus(Guid id, [FromBody] ChangeStatusRequest request, CancellationToken ct)
    {
        return Ok(await cases.ChangeStatusAsync(id, request.Status, ct));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await cases.DeleteAsync(id, ct);
        return NoContent();
    }
}
