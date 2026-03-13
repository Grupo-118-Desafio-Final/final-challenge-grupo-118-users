using Application.Cache;
using Domain.Plan.Dto;
using Domain.Plan.Ports.In;
using Microsoft.AspNetCore.Mvc;

namespace FinalChallengeUsers.API.Controllers;

/// <summary>
/// Controller responsável por expor endpoints HTTP relacionados ao gerenciamento de planos.
/// </summary>
[ApiController]
[Route("plan")]
public class PlanController : ControllerBase
{
    private readonly IPlanManager _planManager;
    private readonly CacheService _cache;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="PlanController"/>.
    /// </summary>
    /// <param name="planManager">Instância de <see cref="IPlanManager"/> usada para operações de plano.</param>
    /// <param name="cache">Serviço de cache distribuído.</param>
    public PlanController(IPlanManager planManager, CacheService cache)
    {
        _planManager = planManager;
        _cache = cache;
    }

    /// <summary>
    /// Retorna um plano pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador do plano.</param>
    /// <returns>
    /// HTTP 200 com os dados do plano, ou HTTP 404 caso não seja encontrado.
    /// </returns>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var cacheKey = $"plan:{id}";

        var result = await _cache.GetOrCreateAsync(cacheKey, () => _planManager.GetById(id), TimeSpan.FromMinutes(2));

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Cria um novo plano com os dados fornecidos.
    /// </summary>
    /// <param name="planCreateRequestDto">Dados do plano a ser criado.</param>
    /// <returns>HTTP 201 em caso de sucesso.</returns>
    [HttpPost]
    [ProducesResponseType(201)]
    public async Task<IActionResult> Create([FromBody] PlanCreateRequestDto planCreateRequestDto)
    {
        await _planManager.CreateAsync(planCreateRequestDto);

        return Created();
    }

    /// <summary>
    /// Retorna o plano associado a um usuário pelo seu identificador.
    /// </summary>
    /// <param name="userId">Identificador do usuário.</param>
    /// <returns>
    /// HTTP 200 com os dados do plano, ou HTTP 404 caso não seja encontrado.
    /// </returns>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPlanByUserId(string userId)
    {
        var cacheKey = $"user_plan:{userId}";

        var plan = await _cache.GetOrCreateAsync(cacheKey, () => _planManager.GetPlanByUserId(userId), TimeSpan.FromMinutes(2));

        if (plan is null)
            return NotFound();

        return Ok(plan);
    }

    /// <summary>
    /// Retorna todos os planos cadastrados.
    /// </summary>
    /// <returns>HTTP 200 com a lista de planos.</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAll()
    {
        var cacheKey = "plans:all";

        var result = await _cache.GetOrCreateAsync(cacheKey, () => _planManager.GetAll(), TimeSpan.FromMinutes(2));

        return Ok(result);
    }
}
