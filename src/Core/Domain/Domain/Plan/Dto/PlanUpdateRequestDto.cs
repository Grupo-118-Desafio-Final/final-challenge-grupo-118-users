using Domain.Plan.ValueObjects;

namespace Domain.Plan.Dto;

public class PlanUpdateRequestDto
{
    public PlanUpdateRequestDto()
    {
        
    }

    public PlanUpdateRequestDto(
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
    public string Threads { get; set; } = "1";
    private static PlanUpdateRequestDto ToDto(Entities.Plan plan)
    {
        return new PlanUpdateRequestDto
        {
            Id = plan.Id,
            Name = plan.Name,
            Price = plan.Price,
            ImageQuality = plan.ImageQuality,
            MaxSizeInMegaBytes = plan.MaxSizeInMegaBytes,
            MaxDurationInSeconds = plan.MaxDurationInSeconds,
            Threads = plan.Threads
        };
    }

    private static Entities.Plan ToEntity(PlanUpdateRequestDto planUpdateRequestDto)
    {
        return new Entities.Plan
        {
            Id = planUpdateRequestDto.Id,
            Name = planUpdateRequestDto.Name,
            Price = planUpdateRequestDto.Price,
            ImageQuality = planUpdateRequestDto.ImageQuality,
            MaxSizeInMegaBytes = planUpdateRequestDto.MaxSizeInMegaBytes,
            MaxDurationInSeconds = planUpdateRequestDto.MaxDurationInSeconds,
            Threads = planUpdateRequestDto.Threads
        };
    }
}
