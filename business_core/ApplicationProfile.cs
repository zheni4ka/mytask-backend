using AutoMapper;
using Core.DTOs;
using Core.Entities;

namespace Core
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<CreateAssignmentModel, Assignment>().ReverseMap();
            
            CreateMap<Assignment, AssignmentDTO>()
                .ForMember(dest => dest.TotalSteps, opt => opt.MapFrom(src => src.Steps != null ? src.Steps.Count() : 0))
                .ForMember(dest => dest.CompletedSteps, opt => opt.MapFrom(src => src.Steps != null ? src.Steps.Count(s => s.IsCompleted) : 0))
                .ReverseMap();

            CreateMap<EditAssignmentModel, Assignment>().ReverseMap();
            CreateMap<AssignmentDTO, CreateAssignmentModel>().ReverseMap();

            CreateMap<CreateCategoryModel, Category>().ReverseMap();
            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<EditCategoryModel, Category>().ReverseMap();

            CreateMap<CreateStepModel, Step>().ReverseMap();
            CreateMap<StepDTO, Step>().ReverseMap();
            CreateMap<EditStepModel, Step>().ReverseMap();
        }
    }
}
