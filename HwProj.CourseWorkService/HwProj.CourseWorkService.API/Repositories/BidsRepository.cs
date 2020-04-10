﻿using HwProj.CourseWorkService.API.Models;
using HwProj.Repositories;

namespace HwProj.CourseWorkService.API.Repositories
{
    public class BidsRepository : CrudRepository<Bid, long>, IBidsRepository
    {
        public BidsRepository(CourseWorkContext context)
            : base(context)
        {
        }
    }
}
