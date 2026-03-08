using Domain.Base.Dto;

namespace UnitTests.Domain;

public class BaseDtoTest
{
    [Fact]
    public void BaseDto_Id_ShouldBeSettable()
    {
        var dto = new BaseDto { Id = 42 };
        Assert.Equal(42, dto.Id);
    }

    [Fact]
    public void BaseDto_CreatedAt_ShouldDefaultToNow()
    {
        var before = DateTime.Now.AddSeconds(-1);
        var dto = new BaseDto();
        var after = DateTime.Now.AddSeconds(1);

        Assert.InRange(dto.CreatedAt, before, after);
    }

    [Fact]
    public void BaseDto_CreatedAt_ShouldBeSettable()
    {
        var date = new DateTime(2024, 6, 15, 10, 30, 0);
        var dto = new BaseDto { CreatedAt = date };
        Assert.Equal(date, dto.CreatedAt);
    }

    [Fact]
    public void BaseDto_UpdatedAt_ShouldBeSettable()
    {
        var date = new DateTime(2025, 1, 1);
        var dto = new BaseDto { UpdatedAt = date };
        Assert.Equal(date, dto.UpdatedAt);
    }

    [Fact]
    public void BaseDto_UpdatedAt_DefaultShouldBeMinValue()
    {
        var dto = new BaseDto();
        Assert.Equal(default, dto.UpdatedAt);
    }

    [Fact]
    public void BaseDto_AllProperties_ShouldBeIndependentlySettable()
    {
        var dto = new BaseDto
        {
            Id = 99,
            CreatedAt = new DateTime(2023, 3, 1),
            UpdatedAt = new DateTime(2024, 3, 1)
        };

        Assert.Equal(99, dto.Id);
        Assert.Equal(new DateTime(2023, 3, 1), dto.CreatedAt);
        Assert.Equal(new DateTime(2024, 3, 1), dto.UpdatedAt);
    }
}
