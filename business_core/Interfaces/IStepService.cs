using Core.DTOs;

namespace Core.Interfaces
{
    public interface IStepService
    {
        Task<StepDTO> GetStepAsync(int id, string userId);
        Task InsertAsync(CreateStepModel step, string userId);
        Task UpdateAsync(EditStepModel step, string userId);
        Task DeleteAsync(int id, string userId);
        Task<IEnumerable<StepDTO>> GetByAssigmentId(int taskId, string userId);
        Task<IEnumerable<StepDTO>> GetIncompleteStepsWithAssignment(int taskId, string userId);
    }
}
