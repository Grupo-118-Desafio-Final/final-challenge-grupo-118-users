namespace Domain.Plan.Ports.Out;

public interface IPlanRepository
{
    Task CreateAsync(Entities.Plan plan);
    Task UpdateAsync(Entities.Plan entity);
    void Delete(Entities.Plan entity);
    Task<Entities.Plan> GetByIdAsync(int id);
    Task<List<Entities.Plan>> GetAll();
    Task<Entities.Plan> GetByNameAsync(string name);
}
