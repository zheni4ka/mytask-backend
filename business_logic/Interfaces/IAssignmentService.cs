using business_logic.DTOs;
using business_logic.DTOs.Assignment;

namespace business_logic.Interfaces
{
    public interface IAssignmentService
    {
        Task<IEnumerable<AssignmentDTO>> GetAll();
        Task<AssignmentDTO> GetAssignmentAsync(int id);
        Task InsertAsync(CreateAssignmentModel assignment);
        Task UpdateAsync(EditAssignmentModel assignment);
        Task DeleteAsync(int id);
    }
}
