using business_logic.DTOs;
using business_logic.Specifications;

namespace business_logic.Interfaces
{
    public interface IStepService
    {
        Task<IEnumerable<StepDTO>> GetAll();
        Task<StepDTO> GetStepAsync(int id);
        Task InsertAsync(CreateStepModel step);
        Task UpdateAsync(EditStepModel step);
        Task DeleteAsync(int id);
        Task<IEnumerable<StepDTO>> GetByAssigmentId(int taskId);
        Task<IEnumerable<StepDTO>> GetIncompleteStepsWithAssignment(int taskId);
    }
}
