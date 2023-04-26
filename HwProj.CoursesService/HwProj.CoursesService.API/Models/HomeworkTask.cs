﻿using System;
using System.Collections.Generic;
using HwProj.Models.CoursesService.ViewModels;
using HwProj.Repositories;

namespace HwProj.CoursesService.API.Models
{
    public class HomeworkTask : IEntity<long>
    {
        public long Id { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }

        public int MaxRating { get; set; }

        public List<Deadline> Deadlines { get; set; } = new();

        public DateTime PublicationDate { get; set; }

        public long HomeworkId { get; set; }
        
        public Homework Homework { get; set; }
    }
}
