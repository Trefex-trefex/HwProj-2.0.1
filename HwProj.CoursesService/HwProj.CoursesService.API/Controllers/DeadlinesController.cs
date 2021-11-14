using System;
using System.Threading.Tasks;
using AutoMapper;
using HwProj.CoursesService.API.Models;
using HwProj.CoursesService.API.Services;
using HwProj.Models.CoursesService.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HwProj.CoursesService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeadlinesController : Controller
    {
        private readonly IDeadlinesService _deadlinesService;
        private readonly IMapper _mapper;

        public DeadlinesController(IDeadlinesService deadlinesService, IMapper mapper)
        {
            _deadlinesService = deadlinesService;
            _mapper = mapper;
        }
        
        [HttpPost("{taskId}/add")]
        public async Task<IActionResult> AddDeadline(long taskId, [FromBody] AddDeadlineViewModel addDeadlineViewModel)
        {
            if (addDeadlineViewModel.DateTime <= DateTime.Now)
                return BadRequest();
            var deadlineId =
                await _deadlinesService.AddDeadlineAsync(taskId, _mapper.Map<Deadline>(addDeadlineViewModel));
            return deadlineId != null ? Ok(deadlineId) : Forbid() as IActionResult;
        }
        
        [HttpGet]
        public async Task<DeadlineViewModel[]> GetAllDeadlines()
        {
            var deadlines = await _deadlinesService.GetAllDeadlinesAsync();
            return _mapper.Map<DeadlineViewModel[]>(deadlines);
        }

        [HttpGet("{taskId}/get")]
        public async Task<DeadlineViewModel[]> GetTaskDeadlines(long taskId)
        {
            var deadlines = await _deadlinesService.GetTaskDeadlinesAsync(taskId);
            return _mapper.Map<DeadlineViewModel[]>(deadlines);
        }
        
        [HttpDelete("{deadlineId}/delete")]
        public async Task DeleteDeadline(long deadlineId)
        {
            await _deadlinesService.DeleteDeadline(deadlineId);
        }
    }
}
