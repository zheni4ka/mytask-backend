using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IAssignmentService
    {
        Task InsertAsync(CreateAssignmentModel model, string userId);
        Task UpdateAsync(EditAssignmentModel model, string userId);
        Task DeleteAsync(int id, string userId);
        Task<AssignmentDTO> GetAssignmentAsync(int id, string userId);
        Task<IEnumerable<AssignmentDTO>> GetAll(string userId);
        Task<PagedResult<AssignmentDTO>> GetPagedAssignmentsAsync(PageParameters pageParameters, string userId);
        Task CheckAndCompleteTaskByStepsAsync(int assignmentId, string userId);
    }
}
