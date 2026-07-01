using AutoMapper;
using business_logic.DTOs;
using business_logic.Entities;
using business_logic.Interfaces;
using business_logic.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

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

        public async Task DeleteAsync(int id)
        {
            await _stepRepository.DeleteByIdAsync(id);
            await _stepRepository.SaveAsync();
        }

        public async Task<IEnumerable<StepDTO>> GetAll()
        {
            return _mapper.Map<IEnumerable<StepDTO>>(await _stepRepository.GetAllAsync());
        }

        public async Task<StepDTO> GetStepAsync(int id)
        {
            var step = await _stepRepository.GetItemBySpecAsync(new StepSpecs.ById(id));
            return _mapper.Map<StepDTO>(step);
        }

        public async Task InsertAsync(CreateStepModel step)
        {
            await _stepRepository.InsertAsync(_mapper.Map<Step>(step));
            await _stepRepository.SaveAsync();
        }

        public async Task UpdateAsync(EditStepModel step)
        {
            _stepRepository.Update(_mapper.Map<Step>(step));
            await _stepRepository.SaveAsync();
        }

        public async Task<IEnumerable<StepDTO>> GetByAssigmentId(int taskId)
        {
            var steps = await _stepRepository.GetListBySpecAsync(new StepSpecs.ByIdWithAssignment(taskId));
            return _mapper.Map<IEnumerable<StepDTO>>(steps);
        }

        public async Task<IEnumerable<StepDTO>> GetIncompleteStepsWithAssignment(int taskId)
        {
            var steps = await _stepRepository.GetListBySpecAsync(new StepSpecs.IncompleteStepsForAssignment(taskId));
            return _mapper.Map<IEnumerable<StepDTO>>(steps);
        }
    }
}
