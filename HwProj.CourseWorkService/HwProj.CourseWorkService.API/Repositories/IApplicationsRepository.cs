﻿using System.Threading.Tasks;
using HwProj.CourseWorkService.API.Models;
using HwProj.Repositories;

namespace HwProj.CourseWorkService.API.Repositories
{
    public interface IApplicationsRepository : ICrudRepository<Application, long>
    {
        Task<Application> GetApplicationAsync(long id);
    }
}
