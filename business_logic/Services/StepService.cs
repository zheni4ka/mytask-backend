using AutoMapper;
using business_logic.DTOs;
using business_logic.Entities;
using business_logic.Interfaces;
using business_logic.Specifications;

namespace business_logic.Services
{
    public class StepService : IStepService
    {
        private readonly IRepository<Step> _stepRepository;
        private readonly IMapper _mapper;

        public StepService(IRepository<Step> stepRepository, IMapper mapper)
        {
            _stepRepository = stepRepository;
            _mapper = mapper;
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var step = await _stepRepository.GetItemBySpecAsync(new StepSpecs.ById(id, userId));

            if (step == null) { throw new KeyNotFoundException("Step not found"); }

            await _stepRepository.DeleteByIdAsync(id);
            await _stepRepository.SaveAsync();
        }

        public async Task<IEnumerable<StepDTO>> GetAll(string userId)
        {
            var steps = await _stepRepository.GetListBySpecAsync(new StepSpecs.All(userId));
            if(!steps.Any()) {
                throw new KeyNotFoundException("No steps found");
            }
            return _mapper.Map<IEnumerable<StepDTO>>(steps);
        }

        public async Task<StepDTO> GetStepAsync(int id, string userId)
        {
            var step = await _stepRepository.GetItemBySpecAsync(new StepSpecs.ById(id, userId));

            if (step == null) { throw new KeyNotFoundException("Step not found"); }

            return _mapper.Map<StepDTO>(step);
        }

        public async Task InsertAsync(CreateStepModel step, string userId)
        {
            var stepEntity = _mapper.Map<Step>(step);

            if(stepEntity.Assignment.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not the owner of this assignment");
            }

            await _stepRepository.InsertAsync(_mapper.Map<Step>(step));
            await _stepRepository.SaveAsync();
        }

        public async Task UpdateAsync(EditStepModel step, string userId)
        {
            var stepEntity = _mapper.Map<Step>(step);

            if(stepEntity.Assignment.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not the owner of this assignment");
            }

            _stepRepository.Update(stepEntity);
            await _stepRepository.SaveAsync();
        }

        public async Task<IEnumerable<StepDTO>> GetByAssigmentId(int taskId, string userId)
        {
            var steps = await _stepRepository.GetListBySpecAsync(new StepSpecs.ByIdWithAssignment(taskId, userId));
            if(!steps.Any()) {
                throw new KeyNotFoundException("No steps found for the given assignment");
            }
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
