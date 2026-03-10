using Microsoft.Extensions.Logging;
using NSubstitute;
using UnitTests.Helpers;
using PlanManager = Application.Plans.PlanManager;
using PlanEntity = Domain.Plan.Entities.Plan;
using PlanCreateRequestDto = Domain.Plan.Dto.PlanCreateRequestDto;
using IPlanRepository = Domain.Plan.Ports.Out.IPlanRepository;
using IUserPlanManager = Domain.UserPlan.Ports.In.IUserPlanManager;
using UserPlanResponse = Domain.UserPlan.Dto.UserPlanResponse;

namespace UnitTests.Application.Plan;

public class PlanManagerEdgeCasesTest
{
    private readonly ILogger<PlanManager> _logger;
    private readonly IPlanRepository _planRepository;
    private readonly IUserPlanManager _userPlanManager;
    private readonly PlanManager _sut;

    public PlanManagerEdgeCasesTest()
    {
        _logger = Substitute.For<ILogger<PlanManager>>();
        _planRepository = Substitute.For<IPlanRepository>();
        _userPlanManager = Substitute.For<IUserPlanManager>();
        _sut = new PlanManager(_logger, _planRepository, _userPlanManager);
    }

    [Fact]
    public async Task DeleteAsync_WhenPlanNotFound_ShouldCallDeleteWithNull()
    {
        // Arrange
        _planRepository.GetByIdAsync(999).Returns((PlanEntity)null!);

        // Act
        await _sut.DeleteAsync(999);

        // Assert
        _planRepository.Received(1).Delete(null!);
    }

    [Fact]
    public async Task GetAll_WhenRepositoryReturnsEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        _planRepository.GetAll().Returns(new List<PlanEntity>());

        // Act
        var result = await _sut.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPlanByUserId_WhenUserPlanIsNull_ShouldThrowNullReferenceException()
    {
        // Arrange
        _userPlanManager.GetByUserId("no-plan-user").Returns((UserPlanResponse)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(
            () => _sut.GetPlanByUserId("no-plan-user"));
    }

    [Fact]
    public async Task GetPlanByUserId_WhenPlanNotFound_ShouldReturnNull()
    {
        // Arrange
        var userPlanResponse = new UserPlanResponse { PlanId = 999, UserId = "user-x" };
        _userPlanManager.GetByUserId("user-x").Returns(userPlanResponse);
        _planRepository.GetByIdAsync(999).Returns((PlanEntity)null!);

        // Act
        var result = await _sut.GetPlanByUserId("user-x");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldCallRepositoryWithCorrectName()
    {
        // Arrange
        _planRepository.GetByNameAsync("Plano Test").Returns((PlanEntity)null!);

        // Act
        await _sut.GetByNameAsync("Plano Test");

        // Assert
        await _planRepository.Received(1).GetByNameAsync("Plano Test");
    }

    [Fact]
    public async Task CreateAsync_ShouldCallRepositoryExactlyOnce()
    {
        // Arrange
        var dto = new PlanCreateRequestDto { Name = "Novo Plano", Price = 15m };

        // Act
        await _sut.CreateAsync(dto);

        // Assert
        await _planRepository.Received(1).CreateAsync(Arg.Any<PlanEntity>());
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrows_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var dto = new PlanCreateRequestDto { Name = "Plano Erro", Price = 10m };
        var expectedException = new InvalidOperationException("Erro no banco de dados");
        _planRepository
            .When(r => r.CreateAsync(Arg.Any<PlanEntity>()))
            .Do(_ => throw expectedException);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(dto));
        _logger.ShouldHaveLoggedError(expectedException);
    }

    [Fact]
    public async Task GetAll_WhenRepositoryThrows_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Erro de conexão");
        _planRepository
            .When(r => r.GetAll())
            .Do(_ => throw expectedException);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(_sut.GetAll);
        _logger.ShouldHaveLoggedError(expectedException);
    }
}
