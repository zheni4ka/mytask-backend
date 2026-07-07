using AutoMapper;
using business_logic.DTOs;
using business_logic.Entities;
using business_logic.Interfaces;
using Hangfire;

namespace business_logic.Helpers
{
    public class RecurringJobHelper
    {
        private readonly IRepository<Assignment> _assignmentRepo;
        private readonly IMapper _mapper;

        public RecurringJobHelper(IRepository<Assignment> assignmentService, IMapper mapper)
        {
            _assignmentRepo = assignmentService;
            _mapper = mapper;
        }

        public async Task GenerateTaskCopyAsync(int originalId)
        {
            var newAssignmentModel = await _assignmentRepo.GetByIdAsync(originalId);

            newAssignmentModel.DueDate = CalculateNextDueDate(newAssignmentModel.DueDate, newAssignmentModel.RefreshType);
            newAssignmentModel.RefreshType = newAssignmentModel.RefreshType;
            newAssignmentModel.IsCompleted = false;
            newAssignmentModel.UserId = newAssignmentModel.UserId;

            await _assignmentRepo.InsertAsync(newAssignmentModel);
        }

        public void RemoveRecurringJob(int assignmentId)
        {
            string jobId = $"Assignment_{assignmentId}";
            RecurringJob.RemoveIfExists(jobId);
        }

        public void ScheduleRecurringJob(int assignmentId, string cronExpression)
        {
            string jobId = $"Assignment_{assignmentId}";
            RecurringJob.AddOrUpdate<RecurringJobHelper>(
                   jobId,
                   job => job.GenerateTaskCopyAsync(assignmentId),
                   cronExpression
               );
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