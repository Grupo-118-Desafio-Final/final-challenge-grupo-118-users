using Domain.Plan.ValueObjects;
using PlanEntity = Domain.Plan.Entities.Plan;

namespace Domain.Plan.Dto;

public class PlanResponseDto
{
    public PlanResponseDto()
    {

    }

    public PlanResponseDto(
        int id,
        string name,
        decimal price
        )
    {
        Id = id;
        Name = name;
        Price = price;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public ImageQualityEnum ImageQuality { get; set; } = ImageQualityEnum.Hd;
    public string MaxSizeInMegaBytes { get; set; } = "200";
    public string MaxDurationInSeconds { get; set; } = "20";
    public int DesiredFrames { get; set; } = 1;

    public static PlanResponseDto ToDto(Entities.Plan plan)
    {
        return new PlanResponseDto
        {
            Id = plan.Id,
            Name = plan.Name,
            Price = plan.Price,
            ImageQuality = plan.ImageQuality,
            MaxSizeInMegaBytes = plan.MaxSizeInMegaBytes,
            MaxDurationInSeconds = plan.MaxDurationInSeconds,
            DesiredFrames = plan.DesiredFrames
        };
    }

    public static PlanEntity ToEntity(PlanResponseDto planResponseDto)
    {
        return new PlanEntity
        {
            Id = planResponseDto.Id,
            Name = planResponseDto.Name,
            Price = planResponseDto.Price,
            ImageQuality = planResponseDto.ImageQuality,
            MaxSizeInMegaBytes = planResponseDto.MaxSizeInMegaBytes,
            MaxDurationInSeconds = planResponseDto.MaxDurationInSeconds,
            DesiredFrames = planResponseDto.DesiredFrames
        };
    }
}
