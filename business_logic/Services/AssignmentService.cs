using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using business_logic.Specifications;
using Hangfire;
using Hangfire.SqlServer;

namespace business_logic.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IRepository<Assignment> _assignmentRepo;
        private readonly IMapper _mapper;

        public AssignmentService(IRepository<Assignment> repository, IMapper mapper)
        {
            this._assignmentRepo = repository;
            this._mapper = mapper;
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var assignment = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.ById(id, userId));
            if (assignment == null)
                throw new KeyNotFoundException("Assignment not found");

            await _assignmentRepo.DeleteByIdAsync(id);
            await _assignmentRepo.SaveAsync();
        }

        public async Task<IEnumerable<AssignmentDTO>> GetAll(string userId)
        {
            var list = await _assignmentRepo.GetAllAsync();
            //if(!list.Any())
            //    throw new KeyNotFoundException("No assignments found");

            return _mapper.Map<IEnumerable<AssignmentDTO>>(list);
        }

        public async Task<AssignmentDTO> GetAssignmentAsync(int id, string userId)
        {
            var obj = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.ById(id, userId));

            //if (obj == null) throw new KeyNotFoundException("Assignment not found");

            return _mapper.Map<AssignmentDTO>(obj);
        }

        public async Task InsertAsync(CreateAssignmentModel model, string userId)
        {
            var assignment = _mapper.Map<Assignment>(model);
            assignment.UserId = userId;

            await _assignmentRepo.InsertAsync(assignment);
            await _assignmentRepo.SaveAsync();
        }

        public async Task UpdateAsync(EditAssignmentModel model, string userId)
        {
            var existingAssignment = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.ById(model.Id, userId));

            if (existingAssignment == null)
                throw new KeyNotFoundException("Assignment not found");

            bool justCompleted = model.IsCompleted && !existingAssignment.IsCompleted;

            _mapper.Map(model, existingAssignment);
            await _assignmentRepo.SaveAsync();

            if (justCompleted && existingAssignment.RefreshType.HasValue)
            {
                await GenerateNextRecurringTaskAsync(existingAssignment, userId);
            }
        }

        public async Task<AssignmentDTO> GetLatestByCategoryId(int categoryId, string userId)
        {
            var assignments = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.LatestByCategoryId(categoryId, userId));

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

            return _mapper.Map<IEnumerable<AssignmentDTO>>(assignments);
        }

        public async Task<IEnumerable<AssignmentDTO>> GetUpcomingAssignments(int daysAhead, string userId)
        {
            var assignments = await _assignmentRepo.GetListBySpecAsync(new AssignmentSpecs.UpcomingAssignments(daysAhead, userId));

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

        private async Task GenerateNextRecurringTaskAsync(Assignment completedAssignment, string userId)
        {
            var nextTask = new Assignment
            {
                Title = completedAssignment.Title,
                Description = completedAssignment.Description,
                CategoryId = completedAssignment.CategoryId,
                UserId = userId,
                RefreshType = completedAssignment.RefreshType,
                IsCompleted = false,
                DueDate = CalculateNextDueDate(completedAssignment.DueDate, completedAssignment.RefreshType.Value)
            };

            await _assignmentRepo.InsertAsync(nextTask);
            await _assignmentRepo.SaveAsync();
        }

        private DateTime CalculateNextDueDate(DateTime currentDueDate, RefreshType refreshType)
        {
            var baseDate = DateTime.UtcNow > currentDueDate ? DateTime.UtcNow : currentDueDate;

            return refreshType switch
            {
                RefreshType.Daily => baseDate.AddDays(1),
                RefreshType.Weekly => baseDate.AddDays(7),
                RefreshType.Monthly => baseDate.AddMonths(1),
                _ => baseDate.AddDays(1)
            };
        }
    }
}
