using PlanEntity = Domain.Plan.Entities.Plan;
using PlanCreateRequestDto = Domain.Plan.Dto.PlanCreateRequestDto;
using PlanResponseDto = Domain.Plan.Dto.PlanResponseDto;
using PlanUpdateRequestDto = Domain.Plan.Dto.PlanUpdateRequestDto;
using ImageQualityEnum = Domain.Plan.ValueObjects.ImageQualityEnum;

namespace UnitTests.Domain.Dto;

public class PlanDtoTest
{
    // --- PlanCreateRequestDto ---

    [Fact]
    public void PlanCreateRequestDto_ToDto_ShouldMapAllFieldsFromPlan()
    {
        var plan = new PlanEntity
        {
            Id = 1,
            Name = "Gold",
            Price = 99.99m,
            ImageQuality = ImageQualityEnum.FullHd,
            MaxSizeInMegaBytes = "500",
            MaxDurationInSeconds = "60",
            Threads = "4"
        };

        var dto = PlanCreateRequestDto.ToDto(plan);

        Assert.Equal("Gold", dto.Name);
        Assert.Equal(99.99m, dto.Price);
        Assert.Equal(ImageQualityEnum.FullHd, dto.ImageQuality);
        Assert.Equal("500", dto.MaxSizeInMegaBytes);
        Assert.Equal("60", dto.MaxDurationInSeconds);
        Assert.Equal("4", dto.Threads);
    }

    [Fact]
    public void PlanCreateRequestDto_ToEntity_ShouldMapAllFieldsFromDto()
    {
        var dto = new PlanCreateRequestDto
        {
            Name = "Silver",
            Price = 49.99m,
            ImageQuality = ImageQualityEnum.UltraHd,
            MaxSizeInMegaBytes = "300",
            MaxDurationInSeconds = "30",
            Threads = "2"
        };

        var plan = PlanCreateRequestDto.ToEntity(dto);

        Assert.Equal("Silver", plan.Name);
        Assert.Equal(49.99m, plan.Price);
        Assert.Equal(ImageQualityEnum.UltraHd, plan.ImageQuality);
        Assert.Equal("300", plan.MaxSizeInMegaBytes);
        Assert.Equal("30", plan.MaxDurationInSeconds);
        Assert.Equal("2", plan.Threads);
    }

    [Fact]
    public void PlanCreateRequestDto_DefaultValues_ShouldBeCorrect()
    {
        var dto = new PlanCreateRequestDto();

        Assert.Equal(ImageQualityEnum.Hd, dto.ImageQuality);
        Assert.Equal("200", dto.MaxSizeInMegaBytes);
        Assert.Equal("20", dto.MaxDurationInSeconds);
        Assert.Equal("1", dto.Threads);
    }

    // --- PlanResponseDto ---

    [Fact]
    public void PlanResponseDto_ToDto_ShouldMapAllFieldsFromPlan()
    {
        var plan = new PlanEntity
        {
            Id = 3,
            Name = "Premium",
            Price = 199m,
            ImageQuality = ImageQualityEnum.FourK,
            MaxSizeInMegaBytes = "1000",
            MaxDurationInSeconds = "120",
            Threads = "8"
        };

        var dto = PlanResponseDto.ToDto(plan);

        Assert.Equal(3, dto.Id);
        Assert.Equal("Premium", dto.Name);
        Assert.Equal(199m, dto.Price);
        Assert.Equal(ImageQualityEnum.FourK, dto.ImageQuality);
        Assert.Equal("1000", dto.MaxSizeInMegaBytes);
        Assert.Equal("120", dto.MaxDurationInSeconds);
        Assert.Equal("8", dto.Threads);
    }

    [Fact]
    public void PlanResponseDto_DefaultConstructor_ShouldHaveDefaultValues()
    {
        var dto = new PlanResponseDto();

        Assert.Equal(0, dto.Id);
        Assert.Null(dto.Name);
        Assert.Equal(0m, dto.Price);
        Assert.Equal(ImageQualityEnum.Hd, dto.ImageQuality);
        Assert.Equal("200", dto.MaxSizeInMegaBytes);
        Assert.Equal("20", dto.MaxDurationInSeconds);
        Assert.Equal("1", dto.Threads);
    }

    [Fact]
    public void PlanResponseDto_ParameterizedConstructor_ShouldSetIdNameAndPrice()
    {
        var dto = new PlanResponseDto(5, "Basic", 9.99m);

        Assert.Equal(5, dto.Id);
        Assert.Equal("Basic", dto.Name);
        Assert.Equal(9.99m, dto.Price);
    }

    // --- PlanUpdateRequestDto ---

    [Fact]
    public void PlanUpdateRequestDto_DefaultConstructor_ShouldHaveDefaultValues()
    {
        var dto = new PlanUpdateRequestDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(ImageQualityEnum.Hd, dto.ImageQuality);
        Assert.Equal("200", dto.MaxSizeInMegaBytes);
        Assert.Equal("20", dto.MaxDurationInSeconds);
        Assert.Equal("1", dto.Threads);
    }

    [Fact]
    public void PlanUpdateRequestDto_ParameterizedConstructor_ShouldSetIdNameAndPrice()
    {
        var dto = new PlanUpdateRequestDto(10, "Pro", 79.99m);

        Assert.Equal(10, dto.Id);
        Assert.Equal("Pro", dto.Name);
        Assert.Equal(79.99m, dto.Price);
    }

    [Fact]
    public void PlanUpdateRequestDto_PropertiesShouldBeSettable()
    {
        var dto = new PlanUpdateRequestDto
        {
            Id = 7,
            Name = "Enterprise",
            Price = 299m,
            ImageQuality = ImageQualityEnum.EightK,
            MaxSizeInMegaBytes = "5000",
            MaxDurationInSeconds = "3600",
            Threads = "16"
        };

        Assert.Equal(7, dto.Id);
        Assert.Equal("Enterprise", dto.Name);
        Assert.Equal(299m, dto.Price);
        Assert.Equal(ImageQualityEnum.EightK, dto.ImageQuality);
        Assert.Equal("5000", dto.MaxSizeInMegaBytes);
        Assert.Equal("3600", dto.MaxDurationInSeconds);
        Assert.Equal("16", dto.Threads);
    }
}
