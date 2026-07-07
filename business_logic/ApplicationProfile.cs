using AutoMapper;
using business_logic.DTOs;
using business_logic.Entities;

namespace business_logic
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
