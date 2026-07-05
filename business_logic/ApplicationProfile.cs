using AutoMapper;
using business_logic.DTOs;
using business_logic.Entities;

namespace business_logic
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            // Assignment maps - маппування Refresh поля на RefreshType в DTO
            CreateMap<CreateAssignmentModel, Assignment>()
                .ForMember(dest => dest.Refresh, opt => opt.MapFrom(src => src.RefreshType))
                .ReverseMap()
                .ForMember(dest => dest.RefreshType, opt => opt.MapFrom(src => src.Refresh));

            CreateMap<AssignmentDTO, Assignment>()
                .ForMember(dest => dest.Refresh, opt => opt.MapFrom(src => src.RefreshType))
                .ReverseMap()
                .ForMember(dest => dest.RefreshType, opt => opt.MapFrom(src => src.Refresh));

            CreateMap<EditAssignmentModel, Assignment>()
                .ForMember(dest => dest.Refresh, opt => opt.MapFrom(src => src.RefreshType))
                .ReverseMap()
                .ForMember(dest => dest.RefreshType, opt => opt.MapFrom(src => src.Refresh));

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
