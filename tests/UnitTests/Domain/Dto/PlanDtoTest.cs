using System.Reflection;
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
            DesiredFrames = 4
        };

        var dto = PlanCreateRequestDto.ToDto(plan);

        Assert.Equal("Gold", dto.Name);
        Assert.Equal(99.99m, dto.Price);
        Assert.Equal(ImageQualityEnum.FullHd, dto.ImageQuality);
        Assert.Equal("500", dto.MaxSizeInMegaBytes);
        Assert.Equal("60", dto.MaxDurationInSeconds);
        Assert.Equal(4, dto.DesiredFrames);
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
            DesiredFrames = 2
        };

        var plan = PlanCreateRequestDto.ToEntity(dto);

        Assert.Equal("Silver", plan.Name);
        Assert.Equal(49.99m, plan.Price);
        Assert.Equal(ImageQualityEnum.UltraHd, plan.ImageQuality);
        Assert.Equal("300", plan.MaxSizeInMegaBytes);
        Assert.Equal("30", plan.MaxDurationInSeconds);
        Assert.Equal(2, plan.DesiredFrames);
    }

    [Fact]
    public void PlanCreateRequestDto_DefaultValues_ShouldBeCorrect()
    {
        var dto = new PlanCreateRequestDto();

        Assert.Equal(ImageQualityEnum.Hd, dto.ImageQuality);
        Assert.Equal("200", dto.MaxSizeInMegaBytes);
        Assert.Equal("20", dto.MaxDurationInSeconds);
        Assert.Equal(1, dto.DesiredFrames);
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
            DesiredFrames = 8
        };

        var dto = PlanResponseDto.ToDto(plan);

        Assert.Equal(3, dto.Id);
        Assert.Equal("Premium", dto.Name);
        Assert.Equal(199m, dto.Price);
        Assert.Equal(ImageQualityEnum.FourK, dto.ImageQuality);
        Assert.Equal("1000", dto.MaxSizeInMegaBytes);
        Assert.Equal("120", dto.MaxDurationInSeconds);
        Assert.Equal(8, dto.DesiredFrames);
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
        Assert.Equal(1, dto.DesiredFrames);
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
        Assert.Equal(1, dto.DesiredFrames);
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
            DesiredFrames = 16
        };

        Assert.Equal(7, dto.Id);
        Assert.Equal("Enterprise", dto.Name);
        Assert.Equal(299m, dto.Price);
        Assert.Equal(ImageQualityEnum.EightK, dto.ImageQuality);
        Assert.Equal("5000", dto.MaxSizeInMegaBytes);
        Assert.Equal("3600", dto.MaxDurationInSeconds);
        Assert.Equal(16, dto.DesiredFrames);
    }

    // --- PlanUpdateRequestDto private methods (via reflection) ---

    [Fact]
    public void PlanUpdateRequestDto_PrivateToDto_ShouldMapAllFieldsFromPlan()
    {
        var plan = new PlanEntity
        {
            Id = 5,
            Name = "Via Reflexão",
            Price = 55m,
            ImageQuality = ImageQualityEnum.FullHd,
            MaxSizeInMegaBytes = "500",
            MaxDurationInSeconds = "60",
            DesiredFrames = 4
        };

        var toDtoMethod = typeof(PlanUpdateRequestDto)
            .GetMethod("ToDto", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(toDtoMethod);
        var result = toDtoMethod!.Invoke(null, new object[] { plan }) as PlanUpdateRequestDto;

        Assert.NotNull(result);
        Assert.Equal(5, result!.Id);
        Assert.Equal("Via Reflexão", result.Name);
        Assert.Equal(55m, result.Price);
        Assert.Equal(ImageQualityEnum.FullHd, result.ImageQuality);
        Assert.Equal("500", result.MaxSizeInMegaBytes);
        Assert.Equal("60", result.MaxDurationInSeconds);
        Assert.Equal(4, result.DesiredFrames);
    }

    [Fact]
    public void PlanUpdateRequestDto_PrivateToEntity_ShouldMapAllFieldsFromDto()
    {
        var dto = new PlanUpdateRequestDto
        {
            Id = 8,
            Name = "Entity Mapeado",
            Price = 88m,
            ImageQuality = ImageQualityEnum.UltraHd,
            MaxSizeInMegaBytes = "800",
            MaxDurationInSeconds = "80",
            DesiredFrames = 8
        };

        var toEntityMethod = typeof(PlanUpdateRequestDto)
            .GetMethod("ToEntity", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(toEntityMethod);
        var result = toEntityMethod!.Invoke(null, new object[] { dto }) as PlanEntity;

        Assert.NotNull(result);
        Assert.Equal(8, result!.Id);
        Assert.Equal("Entity Mapeado", result.Name);
        Assert.Equal(88m, result.Price);
        Assert.Equal(ImageQualityEnum.UltraHd, result.ImageQuality);
        Assert.Equal("800", result.MaxSizeInMegaBytes);
        Assert.Equal("80", result.MaxDurationInSeconds);
        Assert.Equal(8, result.DesiredFrames);
    }
}
