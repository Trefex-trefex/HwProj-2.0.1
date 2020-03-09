﻿using HwProj.CourseWorkService.API.Models;
using HwProj.CourseWorkService.API.Repositories;

namespace HwProj.CourseWorkService.API.Services
{
    public class DeadlineService : EntityService<Deadline>, IDeadlineService
    {
        public DeadlineService(IDeadlinesRepository deadlinesRepository) : base(deadlinesRepository)
        {
        }
    }
}
