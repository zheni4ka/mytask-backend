using business_logic.DTOs;

namespace business_logic.Interfaces
{
    public interface IAssignmentService
    {
        Task InsertAsync(CreateAssignmentModel assignment);
        Task UpdateAsync(EditAssignmentModel assignment);
        Task DeleteAsync(int id);
        Task<AssignmentDTO> GetAssignmentAsync(int id);
        Task<IEnumerable<AssignmentDTO>> GetAll();  
        Task<AssignmentDTO> GetLatestByCategoryId(int categoryId);
        Task<IEnumerable<AssignmentDTO>> GetByCategoryId(int categoryId);
        Task<IEnumerable<AssignmentDTO>> GetOverdueAssignments();
        Task<IEnumerable<AssignmentDTO>> GetUpcomingAssignments(int daysAhead);
        Task<PagedResult<AssignmentDTO>> GetPagedAssignmentsAsync(PageParameters pageParameters);

    }
}
