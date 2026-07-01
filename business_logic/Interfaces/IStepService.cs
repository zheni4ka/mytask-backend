using business_logic.DTOs;

namespace business_logic.Interfaces
{
    public interface IStepService
    {
        Task<IEnumerable<StepDTO>> GetAll();
        Task<StepDTO> GetStepAsync(int id);
        Task InsertAsync(CreateStepModel step);
        Task UpdateAsync(EditStepModel step);
        Task DeleteAsync(int id);
    }
}
