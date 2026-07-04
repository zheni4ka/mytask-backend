using AutoMapper;
using business_logic.DTOs;
using business_logic.Entities;
using business_logic.Interfaces;
using business_logic.Specifications;

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

        public async Task DeleteAsync(int id)
        {
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

        public async Task InsertAsync(CreateAssignmentModel assignment)
        {
            await _assignmentRepo.InsertAsync(_mapper.Map<Assignment>(assignment));
            await _assignmentRepo.SaveAsync();
        }

        public async Task UpdateAsync(EditAssignmentModel assignment)
        {
            _assignmentRepo.Update(_mapper.Map<Assignment>(assignment));
            await _assignmentRepo.SaveAsync();
        }

        public async Task<AssignmentDTO> GetLatestByCategoryId(int categoryId)
        {
            var assignments = await _assignmentRepo.GetItemBySpecAsync(new AssignmentSpecs.LatestByCategoryId(categoryId));
            return _mapper.Map<AssignmentDTO>(assignments);
        }

        public async Task<IEnumerable<AssignmentDTO>> GetByCategoryId(int categoryId)
        {
            var assignments = await _assignmentRepo.GetListBySpecAsync(new AssignmentSpecs.ByCategoryId(categoryId));
            return _mapper.Map<IEnumerable<AssignmentDTO>>(assignments);
        }

        public async Task<IEnumerable<AssignmentDTO>> GetOverdueAssignments()
        {
            var assignments = await _assignmentRepo.GetListBySpecAsync(new AssignmentSpecs.OverdueAssignments());
            return _mapper.Map<IEnumerable<AssignmentDTO>>(assignments);
        }

        public async Task<IEnumerable<AssignmentDTO>> GetUpcomingAssignments(int daysAhead)
        {
            var assignments = await _assignmentRepo.GetListBySpecAsync(new AssignmentSpecs.UpcomingAssignments(daysAhead));
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
    }
}
