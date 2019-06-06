﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using HwProj.CoursesService.API.Models;
using HwProj.CoursesService.API.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HwProj.CoursesService.API.Services
{
    public interface ICoursesService
    {
        Task<Course[]> GetAllAsync();
        Task<Course> GetAsync(long id);
        Task<long> AddAsync(Course course, string mentorId);
        Task DeleteAsync(long id);
        Task UpdateAsync(long courseId, Course updated);
        Task<bool> AddStudentAsync(long courseId, string studentId);
        Task<bool> AcceptCourseMateAsync(long courseId, string studentId);
        Task<bool> RejectCourseMateAsync(long courseId, string studentId);
        long[] GetStudentCourses(string studentId);
        long[] GetMentorCourses(string mentorId);
    }
}