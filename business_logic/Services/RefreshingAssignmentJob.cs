using AutoMapper;
using business_logic.DTOs;
using business_logic.Entities;
using business_logic.Interfaces;

namespace business_logic.Services
{
    public class RefreshingAssignmentJob
    {
        private readonly IAssignmentService _assignmentService;
        private readonly IMapper _mapper;

        public RefreshingAssignmentJob(IAssignmentService assignmentService, IMapper mapper)
        {
            _assignmentService = assignmentService;
            _mapper = mapper;
        }

        public async Task GenerateTaskCopyAsync(int originalId)
        {
            var originalAssignment = await _assignmentService.GetAssignmentAsync(originalId);

            var newAssignmentModel = _mapper.Map<CreateAssignmentModel>(originalAssignment);

            newAssignmentModel.DueDate = CalculateNextDueDate(originalAssignment.DueDate, originalAssignment.RefreshType);
            newAssignmentModel.RefreshType = originalAssignment.RefreshType;
            newAssignmentModel.IsCompleted = false;

            await _assignmentService.InsertAsync(newAssignmentModel, originalAssignment.UserId);
        }

        private DateTime CalculateNextDueDate(DateTime originalDueDate, RefreshType? refreshType)
        {
            return refreshType switch
            {
                RefreshType.Daily => originalDueDate.AddDays(1),
                RefreshType.Weekly => originalDueDate.AddDays(7),
                RefreshType.Monthly => originalDueDate.AddMonths(1),
                _ => DateTime.UtcNow.Date.AddDays(1)
            };
        }
    }
}