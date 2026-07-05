using AutoMapper;
using business_logic.DTOs;
using business_logic.Entities;

namespace business_logic
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<CreateAssignmentModel, Assignment>()
                .ForMember(dest => dest.RefreshType, opt => opt.MapFrom(src => src.RefreshType))
                .ReverseMap()
                .ForMember(dest => dest.RefreshType, opt => opt.MapFrom(src => src.RefreshType));

            CreateMap<AssignmentDTO, Assignment>()
                .ForMember(dest => dest.RefreshType, opt => opt.MapFrom(src => src.RefreshType))
                .ForMember(dest => dest.RefreshType, opt => opt.MapFrom(src => src.RefreshType.ToString()))
                .ReverseMap();
           

            CreateMap<EditAssignmentModel, Assignment>()
                .ForMember(dest => dest.RefreshType, opt => opt.MapFrom(src => src.RefreshType))
                .ReverseMap()
                .ForMember(dest => dest.RefreshType, opt => opt.MapFrom(src => src.RefreshType));

            CreateMap<AssignmentDTO, CreateAssignmentModel>()
                .ForMember(dest => dest.RefreshType, opt => opt.MapFrom(src => src.RefreshType))
                .ReverseMap();

            CreateMap<CreateCategoryModel, Category>().ReverseMap();
            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<EditCategoryModel, Category>().ReverseMap();

            CreateMap<CreateStepModel, Step>().ReverseMap();
            CreateMap<StepDTO, Step>().ReverseMap();
            CreateMap<EditStepModel, Step>().ReverseMap();
        }
    }
}
