﻿using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using HwProj.Models.Roles;
using HwProj.Models.StatisticsService;
using HwProj.SolutionsService.Client;
using HwProj.Utils.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HwProj.APIGateway.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly ISolutionsServiceClient _solutionClient;

        public StatisticsController(ISolutionsServiceClient solutionClient)
        {
            _solutionClient = solutionClient;
        }

        [HttpGet("{courseId}")]
        [ProducesResponseType(typeof(StatisticsCourseMatesModel[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCourseStatistics(long courseId)
        {
            var userId = Request.GetUserId();
            var result = await _solutionClient.GetCourseStatistics(courseId, userId);
            return result == null
                ? Forbid()
                : Ok(result) as IActionResult;
        }
        
        [HttpPost("{courseId}")]
        public async Task<IActionResult> PostCourseStatistics(long courseId, [FromQuery] string spreadSheetId, [FromQuery] string sheetName)
        {
            var userId = Request.GetUserId();
            var result = await _solutionClient.ExportCourseStatistics(courseId, userId, HttpUtility.UrlDecode(spreadSheetId), HttpUtility.UrlDecode(sheetName));
            return result == null
                ? Forbid()
                : Ok(result) as IActionResult;
        }
    }
}