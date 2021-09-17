﻿using AutoMapper;
using HwProj.AuthService.API.Models;
using HwProj.Models.AuthService;
using HwProj.Models.AuthService.DTO;
using HwProj.Models.AuthService.ViewModels;
using HwProj.Models.CoursesService.DTO;

namespace HwProj.AuthService.API
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<RegisterDataDTO, User>();
            CreateMap<EditAccountViewModel, EditDataDTO>();
            CreateMap<RegisterViewModel, RegisterDataDTO>();
            CreateMap<EditExternalViewModel, EditDataDTO>();
        }
    }
}
