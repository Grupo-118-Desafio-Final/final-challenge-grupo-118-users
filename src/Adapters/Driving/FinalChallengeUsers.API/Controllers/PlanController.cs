using Application.Cache;
using Domain.Plan.Dto;
using Domain.Plan.Ports.In;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using System.Text.Json;

namespace FinalChallengeUsers.API.Controllers;

[Authorize]
[ApiController]
[Route("plan")]
public class PlanController : Controller
{
    private readonly IPlanManager _planManager;
    private readonly CacheService _cache;

    public PlanController(IPlanManager planManager, CacheService cache)
    {
        _planManager = planManager;
        _cache = cache;
    }

    [HttpGet("GetById")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId2 = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cacheKey = $"plan:{id}";

        var result = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var plan = await _planManager.GetById(id);
            return plan;
        }, TimeSpan.FromMinutes(10));

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        PlanCreateRequestDto planCreateRequestDto)
    {
        await _planManager.CreateAsync(planCreateRequestDto);

        return Created();
    }

    [HttpGet("GetByUserId")]
    public async Task<IActionResult> GetPlanByUserId(string userId)
    {
        var cacheKey = $"user_plan:{userId}";
        var plan = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var plan = await _planManager.GetPlanByUserId(userId);
            return plan;
        }, TimeSpan.FromMinutes(10));

        return Ok(plan);
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var cacheKey = $"plans:all";
        var result = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var plans = await _planManager.GetAll();
            return plans;
        }, TimeSpan.FromMinutes(10));
        return Ok(result);
    }
}
