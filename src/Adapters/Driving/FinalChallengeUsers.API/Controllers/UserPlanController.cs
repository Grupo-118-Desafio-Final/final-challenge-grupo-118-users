using Domain.UserPlan.Dto;
using Domain.UserPlan.Ports.In;
using Microsoft.AspNetCore.Mvc;

namespace FinalChallengeUsers.API.Controllers;

/// <summary>
/// Controller responsável por expor endpoints HTTP relacionados ao gerenciamento de vínculos entre usuários e planos.
/// </summary>
/// <param name="userPlanManager">Instância de <see cref="IUserPlanManager"/> usada para operações de vínculo usuário-plano.</param>
[ApiController]
[Route("userplan")]
public class UserPlanController(IUserPlanManager userPlanManager) : ControllerBase
{
    private readonly IUserPlanManager _userPlanManager = userPlanManager;

    /// <summary>
    /// Cria um novo vínculo entre um usuário e um plano.
    /// </summary>
    /// <param name="request">Dados do vínculo a ser criado, contendo o identificador do usuário e do plano.</param>
    /// <returns>HTTP 201 em caso de sucesso.</returns>
    [HttpPost]
    [ProducesResponseType(201)]
    public async Task<IActionResult> Create([FromBody] UserPlanCreateRequest request)
    {
        await _userPlanManager.CreateAsync(request);

        return Created();
    }

    /// <summary>
    /// Atualiza o vínculo de um usuário com um plano.
    /// </summary>
    /// <param name="request">Dados atualizados do vínculo.</param>
    /// <returns>HTTP 204 em caso de sucesso.</returns>
    [HttpPut]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Update([FromBody] UserPlanUpdateRequest request)
    {
        await _userPlanManager.UpdateAsync(request);

        return NoContent();
    }

    /// <summary>
    /// Retorna um vínculo usuário-plano pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador do vínculo.</param>
    /// <returns>
    /// HTTP 200 com os dados do vínculo, ou HTTP 404 caso não seja encontrado.
    /// </returns>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var userPlan = await _userPlanManager.GetByIdAsync(id);

        if (userPlan is null)
            return NotFound();

        return Ok(userPlan);
    }

    /// <summary>
    /// Retorna o vínculo de um usuário com seu plano pelo identificador do usuário.
    /// </summary>
    /// <param name="userId">Identificador do usuário.</param>
    /// <returns>
    /// HTTP 200 com os dados do vínculo, ou HTTP 404 caso não seja encontrado.
    /// </returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var userPlan = await _userPlanManager.GetByUserId(userId);

        if (userPlan is null)
            return NotFound();

        return Ok(userPlan);
    }

    /// <summary>
    /// Retorna todos os vínculos usuário-plano cadastrados.
    /// </summary>
    /// <returns>HTTP 200 com a lista de vínculos.</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAll()
    {
        var userPlans = await _userPlanManager.GetAllAsync();

        return Ok(userPlans);
    }

    /// <summary>
    /// Remove um vínculo usuário-plano pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador do vínculo a ser removido.</param>
    /// <returns>HTTP 204 em caso de sucesso.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        await _userPlanManager.DeleteAsync(id);

        return NoContent();
    }
}
