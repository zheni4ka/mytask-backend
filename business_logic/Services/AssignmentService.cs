using AutoMapper;
using business_logic.DTOs;
using business_logic.Entities;
using business_logic.Helpers;
using business_logic.Interfaces;
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

        public async Task DeleteAsync(int id)
        {
            var assignment = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.ById(id));
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

        public async Task<IEnumerable<AssignmentDTO>> GetAll()
        {
            var list = await _assignmentRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<AssignmentDTO>>(list);
        }

        public async Task<AssignmentDTO> GetAssignmentAsync(int id)
        {
            var obj = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.ById(id));

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
            var existingAssignment = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.ById(assignment.Id));

            if (existingAssignment == null)
                throw new KeyNotFoundException("Assignment not found");

            var assignmentEntity = _mapper.Map<Assignment>(assignment);
            assignmentEntity.UserId = userId;
            _assignmentRepo.Update(assignmentEntity);
            await _assignmentRepo.SaveAsync();

            await ManageRecurringJob(assignment.Id, existingAssignment.RefreshType, assignment.RefreshType);
        }

        public async Task<AssignmentDTO> GetLatestByCategoryId(int categoryId)
        {
            var assignments = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.LatestByCategoryId(categoryId));
            return _mapper.Map<AssignmentDTO>(assignments);
        }

        public async Task<IEnumerable<AssignmentDTO>> GetByCategoryId(int categoryId)
        {
            var assignments = await _assignmentRepo.GetListBySpecAsync(new AssignmentSpecs.ByCategoryId(categoryId));

            if(!assignments.Any())
                throw new KeyNotFoundException("No assignments found for the given category");

            return _mapper.Map<IEnumerable<AssignmentDTO>>(assignments);
        }

        public async Task<IEnumerable<AssignmentDTO>> GetOverdueAssignments()
        {
            var assignments = await _assignmentRepo.GetListBySpecAsync(new AssignmentSpecs.OverdueAssignments());

            if(!assignments.Any())
                throw new KeyNotFoundException("No overdue assignments found");

            return _mapper.Map<IEnumerable<AssignmentDTO>>(assignments);
        }

        public async Task<IEnumerable<AssignmentDTO>> GetUpcomingAssignments(int daysAhead)
        {
            var assignments = await _assignmentRepo.GetListBySpecAsync(new AssignmentSpecs.UpcomingAssignments(daysAhead));

            if(!assignments.Any())
                throw new KeyNotFoundException("No upcoming assignments found");

            return _mapper.Map<IEnumerable<AssignmentDTO>>(assignments);
        }

        public async Task<PagedResult<AssignmentDTO>> GetPagedAssignmentsAsync(PageParameters pageParameters)
        {
            var dataSpec = new AssignmentSpecs.AssignmentsByQuerySpec(pageParameters);
            var assignments = await _assignmentRepo.GetListBySpecAsync(dataSpec);

            var countSpec = new AssignmentSpecs.AssignmentsByQueryCountSpec(pageParameters);
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

        private async Task ManageRecurringJob(int assignmentId, RefreshType? oldRefreshType, RefreshType? newRefreshType)
        {
            string jobId = $"Assignment_{assignmentId}";
            var assignment = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.ById(assignmentId));

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
