﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HwProj.Models.AuthService.DTO;

namespace HwProj.Models.CoursesService.ViewModels
{
    public class CreateCourseViewModel
    {
        [Required]
        [RegularExpression(@"^\S+.*", ErrorMessage = "Name shouldn't start with white spaces.")]
        public string Name { get; set; }

        public string GroupName { get; set; }

        [Required] public bool IsOpen { get; set; }
    }

    public class UpdateCourseViewModel
    {
        [Required]
        [RegularExpression(@"^\S+.*", ErrorMessage = "Name shouldn't start with white spaces.")]
        public string Name { get; set; }

        public string GroupName { get; set; }

        [Required] public bool IsOpen { get; set; }

        public bool IsCompleted { get; set; }
    }

    public class CourseViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public bool IsOpen { get; set; }
        public bool IsCompleted { get; set; }

        // TODO: исправить
        public string MentorIds { get; set; }
        public string InviteCode { get; set; }
        public List<CourseMateViewModel> CourseMates { get; set; } = new();
        public List<HomeworkViewModel> Homeworks { get; set; } = new();
    }

    // Модель для списка всех курсов
    public class CoursePreview
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public bool IsCompleted { get; set; }
        public string[] MentorIds { get; set; }
    }
}
