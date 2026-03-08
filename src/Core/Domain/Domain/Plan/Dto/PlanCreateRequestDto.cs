using Domain.Plan.ValueObjects;

namespace Domain.Plan.Dto;

public class PlanCreateRequestDto
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public ImageQualityEnum ImageQuality { get; set; } = ImageQualityEnum.Hd;
    public string MaxSizeInMegaBytes { get; set; } = "200";
    public string MaxDurationInSeconds { get; set; } = "20";
    public string Threads { get; set; } = "1";
    public static PlanCreateRequestDto ToDto(Entities.Plan plan)
    {
        return new PlanCreateRequestDto
        {
            Name = plan.Name,
            Price = plan.Price,
            ImageQuality = plan.ImageQuality,
            MaxSizeInMegaBytes = plan.MaxSizeInMegaBytes,
            MaxDurationInSeconds = plan.MaxDurationInSeconds,
            Threads = plan.Threads
        };
    }

    public static Entities.Plan ToEntity(PlanCreateRequestDto request)
    {
        return new Entities.Plan
        {
            Name = request.Name,
            Price = request.Price,
            ImageQuality = request.ImageQuality,
            MaxSizeInMegaBytes = request.MaxSizeInMegaBytes,
            MaxDurationInSeconds = request.MaxDurationInSeconds,
            Threads = request.Threads
        };
    }
}
