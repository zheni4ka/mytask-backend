using AutoMapper;
using business_logic.DTOs;
using business_logic.DTOs.Assignment;
using business_logic.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace business_logic
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<CreateAssignmentModel, Assignment>().ReverseMap();
            CreateMap<AssignmentDTO, Assignment>().ReverseMap();
            CreateMap<EditAssignmentModel, Assignment>().ReverseMap();

            CreateMap<CreateCategoryModel, Category>().ReverseMap();
            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<EditCategoryModel, Category>().ReverseMap();
        }
    }
}
