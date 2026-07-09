using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using business_logic.Helpers;
using business_logic.Specifications;
using Hangfire;
using Hangfire.SqlServer;

namespace business_logic.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IRepository<Assignment> _assignmentRepo;
        private readonly IMapper _mapper;
        private readonly RecurringJobHelper _recurringJobService;

        public AssignmentService(IRepository<Assignment> repository, IMapper mapper, RecurringJobHelper recurringJobService)
        {
            this._assignmentRepo = repository;
            this._mapper = mapper;
            this._recurringJobService = recurringJobService;
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var assignment = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.ById(id, userId));
            if(assignment == null)
                throw new KeyNotFoundException("Assignment not found");

            try
            {
                _recurringJobService.RemoveRecurringJob(id);
            }
            catch
            {
                throw new KeyNotFoundException("Recurring job not found for the assignment");
            }

            await _assignmentRepo.DeleteByIdAsync(id);
            await _assignmentRepo.SaveAsync();
        }

        public async Task<IEnumerable<AssignmentDTO>> GetAll(string userId)
        {
            var list = await _assignmentRepo.GetAllAsync();
            if(!list.Any())
                throw new KeyNotFoundException("No assignments found");

            return _mapper.Map<IEnumerable<AssignmentDTO>>(list);
        }

        public async Task<AssignmentDTO> GetAssignmentAsync(int id, string userId)
        {
            var obj = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.ById(id, userId));

            if (obj == null) throw new KeyNotFoundException("Assignment not found");

            return _mapper.Map<AssignmentDTO>(obj);
        }

        public async Task InsertAsync(CreateAssignmentModel model, string userId)
        {
            var assignment = new Assignment
            {
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                CategoryId = model.CategoryId,
                UserId = userId,
                RefreshType = model.RefreshType
            };

            await _assignmentRepo.InsertAsync(assignment);
            await _assignmentRepo.SaveAsync();

            if (model.RefreshType.HasValue)
            {
                string cron = await GetCronExpression(_mapper.Map<AssignmentDTO>(assignment));

                _recurringJobService.ScheduleRecurringJob(assignment.Id, cron);
            }
        }

        public async Task UpdateAsync(EditAssignmentModel assignment, string userId)
        {
            var existingAssignment = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.ById(assignment.Id, userId));

            if (existingAssignment == null)
                throw new KeyNotFoundException("Assignment not found");

            var assignmentEntity = _mapper.Map<Assignment>(assignment);
            assignmentEntity.UserId = userId;
            _assignmentRepo.Update(assignmentEntity);
            await _assignmentRepo.SaveAsync();

            await ManageRecurringJob(assignment.Id, userId, existingAssignment.RefreshType, assignment.RefreshType);
        }

        public async Task<AssignmentDTO> GetLatestByCategoryId(int categoryId, string userId)
        {
            var assignments = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.LatestByCategoryId(categoryId, userId));
            if (assignments == null)
                throw new KeyNotFoundException("No assignments found for the given category");

            return _mapper.Map<AssignmentDTO>(assignments);
        }

        public async Task<IEnumerable<AssignmentDTO>> GetByCategoryId(int categoryId, string userId)
        {
            var assignments = await _assignmentRepo.GetListBySpecAsync(new AssignmentSpecs.ByCategoryId(categoryId, userId));

            return _mapper.Map<IEnumerable<AssignmentDTO>>(assignments);
        }

        public async Task<IEnumerable<AssignmentDTO>> GetOverdueAssignments(string userId)
        {
            var assignments = await _assignmentRepo.GetListBySpecAsync(new AssignmentSpecs.OverdueAssignments(userId));

            if(!assignments.Any())
                throw new KeyNotFoundException("No overdue assignments found");

            return _mapper.Map<IEnumerable<AssignmentDTO>>(assignments);
        }

        public async Task<IEnumerable<AssignmentDTO>> GetUpcomingAssignments(int daysAhead, string userId)
        {
            var assignments = await _assignmentRepo.GetListBySpecAsync(new AssignmentSpecs.UpcomingAssignments(daysAhead, userId));

            if(!assignments.Any())
                throw new KeyNotFoundException("No upcoming assignments found");

            return _mapper.Map<IEnumerable<AssignmentDTO>>(assignments);
        }

        public async Task<PagedResult<AssignmentDTO>> GetPagedAssignmentsAsync(PageParameters pageParameters, string userId)
        {
            var dataSpec = new AssignmentSpecs.AssignmentsByQuerySpec(pageParameters, userId);
            var assignments = await _assignmentRepo.GetListBySpecAsync(dataSpec);

            var countSpec = new AssignmentSpecs.AssignmentsByQueryCountSpec(pageParameters, userId);
            var totalCount = await _assignmentRepo.CountAsync(countSpec);

            var list = _mapper.Map<IEnumerable<AssignmentDTO>>(assignments);

            return new PagedResult<AssignmentDTO>
            {
                TotalCount = totalCount,
                PageNumber = pageParameters.pageNumber,
                PageSize = pageParameters.pageSize,
                Items = list
            };
        }

        private async Task ManageRecurringJob(int assignmentId, string userId, RefreshType? oldRefreshType, RefreshType? newRefreshType)
        {
            string jobId = $"Assignment_{assignmentId}";
            var assignment = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.ById(assignmentId, userId));

            if (oldRefreshType == null && newRefreshType.HasValue)
            {
                string cron = await GetCronExpression(_mapper.Map<AssignmentDTO>(assignment));
                _recurringJobService.ScheduleRecurringJob(assignmentId, cron);
            }
            else if (oldRefreshType.HasValue && newRefreshType.HasValue && oldRefreshType != newRefreshType)
            {
                string cron = await GetCronExpression(_mapper.Map<AssignmentDTO>(assignment));
                _recurringJobService.ScheduleRecurringJob(assignmentId, cron);
            }
            else if (oldRefreshType.HasValue && newRefreshType == null)
            {
                _recurringJobService.RemoveRecurringJob(assignmentId);
            }
        }

        async Task<string> GetCronExpression(AssignmentDTO assignmentDTO)
        {
            return assignmentDTO.RefreshType switch
            {
                RefreshType.Daily => Hangfire.Cron.Daily(assignmentDTO.DueDate.Hour, assignmentDTO.DueDate.Minute),
                RefreshType.Weekly => Hangfire.Cron.Weekly(assignmentDTO.DueDate.DayOfWeek, assignmentDTO.DueDate.Hour, assignmentDTO.DueDate.Minute),
                RefreshType.Monthly => Hangfire.Cron.Monthly(assignmentDTO.DueDate.Day, assignmentDTO.DueDate.Hour, assignmentDTO.DueDate.Minute),
                _ => throw new ArgumentException("Unknown refresh type")
            };
        }
    }
}
