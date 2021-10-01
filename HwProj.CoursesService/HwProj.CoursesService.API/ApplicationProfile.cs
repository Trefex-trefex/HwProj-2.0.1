﻿using AutoMapper;
using HwProj.CoursesService.API.Models;
using System.Linq;
using HwProj.Models.CoursesService.DTO;
using HwProj.Models.CoursesService.ViewModels;
using System;

namespace HwProj.CoursesService.API
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<Course, UserCourseDescription>();
            CreateMap<Course, CourseViewModel>().ReverseMap();
            CreateMap<Course, CreateCourseViewModel>().ReverseMap();
            CreateMap<Course, UpdateCourseViewModel>().ReverseMap();

            CreateMap<Group, UserGroupDescription>();
            CreateMap<Group, GroupViewModel>()
                .ForMember("GroupMates", cm => cm.MapFrom(g => g.GroupMates.Select(c => new GroupMateViewModel { StudentId = c.StudentId }).ToList()));
            CreateMap<UserGroupDescription, Group>().ReverseMap()
                .ForMember("GroupMates", cm => cm.MapFrom(g => g.GroupMates.Select(c => new GroupMateViewModel { StudentId = c.StudentId }).ToList()));
            CreateMap<Group, CreateGroupViewModel>().ReverseMap()
                .ForMember("GroupMates", cm => cm.MapFrom(g => g.GroupMates.Select(c => new GroupMate { StudentId = c.StudentId }).ToList()));
            CreateMap<UpdateGroupViewModel, Group>()
                .ForMember("GroupMates", cm => cm.MapFrom(g => g.GroupMates.Select(c => new GroupMate { StudentId = c.StudentId }).ToList()));

            CreateMap<GroupMate, GroupMateViewModel>();

            CreateMap<CourseMate, CourseMateViewModel>();

            CreateMap<CreateHomeworkViewModel, Homework>();
            CreateMap<Homework, HomeworkViewModel>();
            CreateMap<HomeworkTask, HomeworkTaskViewModel>().ReverseMap();
            CreateMap<HomeworkTask, HomeworkTaskViewModel>()
                .ForMember("IsDeferred", cm => cm.MapFrom(g => DateTime.UtcNow.AddHours(3) < g.PublicationDate));
            CreateMap<CreateTaskViewModel, HomeworkTask>().ReverseMap();
        }
    }
}
