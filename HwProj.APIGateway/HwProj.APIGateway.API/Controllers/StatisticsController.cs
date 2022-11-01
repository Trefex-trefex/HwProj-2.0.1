﻿using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HwProj.APIGateway.API.ExportServices;
using HwProj.APIGateway.API.Models;
using HwProj.APIGateway.API.TableGenerators;
using HwProj.AuthService.Client;
using HwProj.CoursesService.Client;
using HwProj.Models.Result;
using HwProj.SolutionsService.Client;
using Microsoft.AspNetCore.Mvc;

namespace HwProj.APIGateway.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : AggregationController
    {
        private readonly ISolutionsServiceClient _solutionClient;
        private readonly ICoursesServiceClient _coursesClient;
        private readonly GoogleService _googleService;

        public StatisticsController(
            ISolutionsServiceClient solutionClient,
            ICoursesServiceClient coursesServiceClient,
            IAuthServiceClient authServiceClient,
            GoogleService googleService)
            : base(authServiceClient)
        {
            _solutionClient = solutionClient;
            _coursesClient = coursesServiceClient;
            _googleService = googleService;
        }

        [HttpGet("{courseId}")]
        [ProducesResponseType(typeof(StatisticsCourseMatesModel[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCourseStatistics(long courseId)
        {
            var result = await GetStatistics(courseId);
            if (result == null)
            {
                return Forbid();
            }

            return Ok(result);
        }

        private async Task<IOrderedEnumerable<StatisticsCourseMatesModel>?> GetStatistics(long courseId)
        {
            var statistics = await _solutionClient.GetCourseStatistics(courseId, UserId);
            if (statistics == null) return null;

            var studentIds = statistics.Select(t => t.StudentId).ToArray();
            var students = await AuthServiceClient.GetAccountsData(studentIds);

            var result
                = statistics.Zip(students, (stats, student)
                    => new StatisticsCourseMatesModel
                    {
                        Id = student.UserId,
                        Name = student.Name,
                        Surname = student.Surname,
                        Homeworks = stats.Homeworks
                    }).OrderBy(t => t.Surname).ThenBy(t => t.Name);

            return result;
        }

        /// <summary>
        /// Implements file download.
        /// </summary>
        /// <param name="courseId">The course Id the report is based on.</param>
        /// <param name="userId">Id of the user requesting the report.</param>
        /// <param name="sheetName">Name of the sheet on which the report will be generated.</param>
        /// <returns>File download process.</returns>
        [HttpGet("getFile")]
        public async Task<IActionResult> GetFile(long courseId, string userId, string sheetName)
        {
            var course = await _coursesClient.GetCourseById(courseId, userId);
            var statistics = await GetStatistics(courseId);
            if (statistics == null || course == null) return Forbid();

            var statisticStream =
                await ExcelGenerator.Generate(statistics.ToList(), course, sheetName).GetAsByteArrayAsync();
            return new FileContentResult(statisticStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [HttpGet("getSheetTitles")]
        public async Task<Result<string[]>> GetSheetTitles(string sheetUrl)
            => await _googleService.GetSheetTitles(sheetUrl);

        [HttpPost("processLink")]
        public Result ProcessLink(string? sheetUrl)
        {
            if (sheetUrl == null) return Result.Failed("Некорректная ссылка");
            if (GoogleService.ParseLink(sheetUrl).Succeeded) return Result.Success();
            return Result.Failed("Некорректная ссылка");
        }

        /// <summary>
        /// Implements sending a report to the Google Sheets.
        /// </summary>
        /// <param name="courseId">The course Id the report is based on.</param>
        /// <param name="userId">Id of the user requesting the report.</param>
        /// <param name="sheetUrl">Sheet Url parameter, required to make requests to the Google Sheets.</param>
        /// <param name="sheetName">Sheet Name parameter, required to make requests to the Google Sheets.</param>
        /// <returns>Operation status.</returns>
        [HttpGet("exportToSheet")]
        public async Task<Result> ExportToGoogleSheets(
            long courseId, string userId, string sheetUrl, string sheetName)
        {
            var course = await _coursesClient.GetCourseById(courseId, userId);
            var statistics = await GetStatistics(courseId);
            if (course == null || statistics == null) return Result.Failed("Ошибка при получении статистики");
            var result = await _googleService.Export(course, statistics, sheetUrl, sheetName);
            return result;
        }
    }
}
