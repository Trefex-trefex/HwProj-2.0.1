﻿using HwProj.Repositories;

namespace HwProj.CourseWorkService.API.Models
{
    public class Bid : IEntity<long>
    {
        public long Id { get; set; }

        public string BidValue { get; set; }

        public long ReviewerId { get; set; }
        public User Reviewer { get; set; }

        public long CourseWorkId { get; set; }
        public CourseWork CourseWork { get; set; }

    }
}
