﻿using System.Threading.Tasks;
using HwProj.Models.SolutionsService;
using HwProj.Models.StatisticsService;
using HwProj.SolutionsService.API.AssessmentSystem;
using Microsoft.AspNetCore.Http;

namespace HwProj.SolutionsService.Client
{
    public interface ISolutionsServiceClient
    { 
        Task<Solution[]> GetAllSolutions();
        Task<Solution> GetSolutionById(long solutionId);
        Task<Solution[]> GetUserSolution(long taskId, string studentId);
        Task<long> PostSolution(SolutionViewModel model, long taskId);
        Task RateSolution(long solutionId, int newRating, string lecturerComment, string lecturerId);
        Task MarkSolution(long solutionId);
        Task DeleteSolution(long solutionId);
        Task<long> PostGroupSolution(SolutionViewModel model, long taskId, long groupId);
        Task<Solution[]> GetTaskSolutions(long groupId, long taskId);
        Task<StatisticsCourseMatesModel[]> GetCourseStatistics(long courseId, string userId);
        Task<ResponseForAddAssessmentMethod> AddDllForAssessment(long courseId, IFormFile dll);
        Task<FinalAssessmentForStudent[]> GetAssessmentForCourse(long courseId, string userId);
    }
}
