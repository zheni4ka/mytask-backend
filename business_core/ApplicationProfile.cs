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
            CreateMap<AssignmentDTO, Assignment>().ReverseMap();
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
