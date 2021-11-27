﻿using System.Threading.Tasks;
using HwProj.Models.SolutionsService;
using HwProj.Models.StatisticsService;
using HwProj.SolutionsService.API.AssessmentSystem;
using HwProj.SolutionsService.API.Models;
using Microsoft.AspNetCore.Http;

namespace HwProj.SolutionsService.API.Services
{
    public interface ISolutionsService
    {
        Task<Solution[]> GetAllSolutionsAsync();

        Task<Solution> GetSolutionAsync(long solutionId);

        Task<Solution[]> GetTaskSolutionsFromStudentAsync(long taskId, string studentId);

        Task<Solution[]> GetTaskSolutionsFromGroupAsync(long taskId, long groupId);
        
        Task<long> PostOrUpdateAsync(long taskId, Solution solution);

        Task RateSolutionAsync(long solutionId, int newRating, string lecturerComment);

        Task DeleteSolutionAsync(long solutionId);
        
        Task MarkSolutionFinal(long solutionId);

        Task<ResponseForAddAssessmentMethod> AddDllForAssessment(long courseId, IFormFile dll);

        Task<FinalAssessmentForStudent[]> GetAssessmentForCourseForAllStudents(long courseId, string userId);
    }
}
