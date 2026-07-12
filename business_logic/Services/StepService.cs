using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using business_logic.Specifications;

namespace business_logic.Services
{
    public class StepService : IStepService
    {
        private readonly IRepository<Step> _stepRepository;
        private readonly IRepository<Assignment> _assignmentRepository;
        private readonly IMapper _mapper;


        public StepService(IRepository<Step> stepRepository, IRepository<Assignment> assignmentRepository, IMapper mapper)
        {
            _stepRepository = stepRepository;
            _assignmentRepository = assignmentRepository;
            _mapper = mapper;
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var step = await _stepRepository.GetItemBySpecAsync(new StepSpecs.ById(id, userId));

            if (step == null) { throw new KeyNotFoundException("Step not found"); }

            await _stepRepository.DeleteByIdAsync(id);
            await _stepRepository.SaveAsync();
        }


        public async Task<StepDTO> GetStepAsync(int id, string userId)
        {
            var step = await _stepRepository.GetItemBySpecAsync(new StepSpecs.ById(id, userId));

            if (step == null) { throw new KeyNotFoundException("Step not found"); }

            return _mapper.Map<StepDTO>(step);
        }

        public async Task InsertAsync(CreateStepModel step, string userId)
        {
            var assignment = await _assignmentRepository.GetItemBySpecAsync(new AssignmentSpecs.ById(step.AssignmentId, userId));

            if (assignment == null)
            {
                throw new UnauthorizedAccessException("Task not found or you are not the owner");
            }

            var stepEntity = _mapper.Map<Step>(step);

            await _stepRepository.InsertAsync(stepEntity);
            await _stepRepository.SaveAsync();
        }

        public async Task UpdateAsync(EditStepModel step, string userId)
        {
            var existingStep = await _stepRepository.GetItemBySpecAsync(new StepSpecs.ByIdWithAssignment(step.Id, userId));

            if (existingStep == null)
            {
                throw new KeyNotFoundException("Step not found or access denied");
            }

            _mapper.Map(step, existingStep);

            await _stepRepository.SaveAsync();
        }

        public async Task<IEnumerable<StepDTO>> GetByAssigmentId(int taskId, string userId)
        {
            var steps = await _stepRepository.GetListBySpecAsync(new StepSpecs.ByAssignmentId(taskId, userId));

            return _mapper.Map<IEnumerable<StepDTO>>(steps);
        }

        public async Task<IEnumerable<StepDTO>> GetIncompleteStepsWithAssignment(int taskId, string userId)
        {
            var steps = await _stepRepository.GetListBySpecAsync(new StepSpecs.IncompleteStepsForAssignment(taskId, userId));
            if(!steps.Any()) {
                throw new KeyNotFoundException("No incomplete steps found for the given assignment");
            }
            return _mapper.Map<IEnumerable<StepDTO>>(steps);
        }
    }
}
