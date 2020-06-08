﻿using AutoMapper;
using HwProj.CourseWorkService.API.Models;
using HwProj.CourseWorkService.API.Models.DTO;
using HwProj.CourseWorkService.API.Models.ViewModels;

namespace HwProj.CourseWorkService.API
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap< CourseWork, OverviewCourseWorkDTO>();
            CreateMap<CourseWork, DetailCourseWorkDTO>();
            CreateMap<CourseWork, CreateCourseWorkViewModel>().ReverseMap();
            CreateMap<Application, StudentApplicationDTO>();
            CreateMap<Application, LecturerApplicationDTO>();
            CreateMap<Application, OverviewApplicationDTO>();
            CreateMap<Application, CreateApplicationViewModel>().ReverseMap();
            CreateMap<Deadline, DeadlineDTO>();
            CreateMap<Deadline, AddDeadlineViewModel>().ReverseMap();

            CreateMap<Bid, CreateBidViewModel>().ReverseMap();
            CreateMap<User, ReviewerDTO>();
        }
    }
}
