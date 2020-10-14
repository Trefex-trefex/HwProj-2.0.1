﻿using Microsoft.EntityFrameworkCore;

namespace HwProj.CourseWorkService.API.Models
{
    public sealed class CourseWorkContext : DbContext
    {
        public DbSet<CourseWork> CourseWorks { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Deadline> Deadlines { get; set; }
        public DbSet<WorkFile> WorkFiles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<LecturerProfile> LecturerProfiles { get; set; }
        public DbSet<ReviewerProfile> ReviewerProfiles { get; set; }
        public DbSet<CuratorProfile> CuratorProfiles { get; set; }

        public CourseWorkContext(DbContextOptions options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleName = RoleNames.Student },
                new Role { Id = 2, RoleName = RoleNames.Lecturer }, 
                new Role { Id = 3, RoleName = RoleNames.Reviewer },
                new Role { Id = 4, RoleName = RoleNames.Curator });

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        }
    }
}
