using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using HwProj.AuthService.Client;
using HwProj.CoursesService.Client;
using HwProj.Models.AuthService.ViewModels;
using HwProj.Models.CoursesService.ViewModels;
using HwProj.Models.SolutionsService;
using HwProj.SolutionsService.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using HwProj.Exceptions;

namespace HwProj.SolutionsService.IntegrationTests
{
    [TestFixture]
    public class SolutionsServiceTests
    {
        private RegisterViewModel GenerateRegisterViewModel()
        {
            var password = new Fixture().Create<string>();
            var fixture = new Fixture().Build<RegisterViewModel>()
                .With(vm => vm.Password, password)
                .With(vm => vm.PasswordConfirm, password);
            var viewModel = fixture.Create();
            viewModel.Email += "@mail.ru";
            return viewModel;
        }

        private AuthServiceClient CreateAuthServiceClient()
        {
            var mockIConfiguration = new Mock<IConfiguration>();
            mockIConfiguration.Setup(x => x.GetSection("Services")["Auth"]).Returns("http://localhost:5001");
            var mockClientFactory = new Mock<IHttpClientFactory>();
            mockClientFactory.Setup(x => x.CreateClient(Options.DefaultName)).Returns(new HttpClient());
            return new AuthServiceClient(mockClientFactory.Object, mockIConfiguration.Object);
        }

        private CreateCourseViewModel GenerateCreateCourseViewModel()
        {
            var fixture = new Fixture().Build<CreateCourseViewModel>()
                .With(cvm => cvm.IsOpen, true);
            return fixture.Create();
        }
        
        private CreateHomeworkViewModel GenerateCreateHomeworkViewModel()
        {
            var fixture = new Fixture().Build<CreateHomeworkViewModel>()
                .With(hvm => hvm.Tasks, new List<CreateTaskViewModel>())
                .With(hvm => hvm.Tasks, new List<CreateTaskViewModel>());
            return fixture.Create();
        }

        private CreateTaskViewModel GenerateCreateTaskViewModelWithoutDeadLine()
        {
            return new Fixture().Build<CreateTaskViewModel>()
                .With(t  => t.HasDeadline, false)
                .Create();
        }
        
        private CreateTaskViewModel GenerateCreateTaskViewModelWithDeadLine()
        {
            return new Fixture().Build<CreateTaskViewModel>()
                .With(t => t.HasDeadline, true)
                .With(t => t.DeadlineDate, DateTime.Now)
                .Create();
        }
        
        private CoursesServiceClient CreateCourseServiceClient(string userId)
        {
            var mockIConfiguration = new Mock<IConfiguration>();
            mockIConfiguration.Setup(x => x.GetSection("Services")["Courses"]).Returns("http://localhost:5002");
            var mockClientFactory = new Mock<IHttpClientFactory>();
            mockClientFactory.Setup(x => x.CreateClient(Options.DefaultName)).Returns(new HttpClient());
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(x => x.HttpContext.User.FindFirst("_id")).Returns(new Claim("", userId));
            return new CoursesServiceClient(mockClientFactory.Object, mockHttpContextAccessor.Object,
                mockIConfiguration.Object);
        }
        
        private async Task<(string, string)> CreateAndRegisterUser()
        {
            var authClient = CreateAuthServiceClient();
            var userData = GenerateRegisterViewModel();
            await authClient.Register(userData);
            var userId = await authClient.FindByEmailAsync(userData.Email);
            return (userId, userData.Email);
        }

        private async Task<(string, string)> CreateAndRegisterLecture()
        {
            var (userId, mail) = await CreateAndRegisterUser();
            var authClient = CreateAuthServiceClient();
            await authClient.InviteNewLecturer(new InviteLecturerViewModel() {Email = mail});
            return (userId, mail);
        }

        private async Task<long> CreateCourse(CoursesServiceClient courseClient, string userId)
        {
            var newCourseViewModel = GenerateCreateCourseViewModel();
            var courseId = await courseClient.CreateCourse(newCourseViewModel, userId);
            return courseId;
        }

        private async Task SignStudentInCourse(
            CoursesServiceClient studentCourseClient,
            CoursesServiceClient lectureCourseClient,
            long courseId,
            string studentId)
        {
            await studentCourseClient.SignInCourse(courseId, studentId);
            await lectureCourseClient.AcceptStudent(courseId, studentId);
        }

        private SolutionViewModel GenerateSolutionViewModel(string userId)
        {
            var url = new Fixture().Create<string>();
            var fixture = new Fixture().Build<SolutionViewModel>()
                .With(h => h.GithubUrl, url)
                .With(h => h.StudentId, userId);
            var viewModel = fixture.Create();
            return viewModel;
        }
        
        private SolutionsServiceClient CreateSolutionsServiceClient()
        {
            var mockIConfiguration = new Mock<IConfiguration>();
            mockIConfiguration.Setup(x => x.GetSection("Services")["Solutions"]).Returns("http://localhost:5007");
            var mockClientFactory = new Mock<IHttpClientFactory>();
            mockClientFactory.Setup(x => x.CreateClient(Options.DefaultName)).Returns(new HttpClient());
            return new SolutionsServiceClient(mockClientFactory.Object, mockIConfiguration.Object);
        }

        private async Task<(string, string)> CreateUserAndLecture()
        {
            var (studentId, _) = await CreateAndRegisterUser();
            var (lectureId, _) = await CreateAndRegisterLecture();
            return (studentId, lectureId);
        }

        private async Task<(long, long, long)> CreateCourseHomeworkTaskWithOutDeadLine(CoursesServiceClient lectureCourseClient, string lectureId)
        {
            var courseId = await CreateCourse(lectureCourseClient, lectureId);
            var newHomeworkViewModel = GenerateCreateHomeworkViewModel();
            var newTaskViewModel = GenerateCreateTaskViewModelWithoutDeadLine();
            var homeworkId = await lectureCourseClient.AddHomeworkToCourse(newHomeworkViewModel, courseId);
            var taskId = await lectureCourseClient.AddTask(newTaskViewModel, homeworkId);
            return (courseId, homeworkId, taskId);
        }
        
        private async Task<(long, long, long)> CreateCourseHomeworkTaskWithDeadLine(CoursesServiceClient lectureCourseClient, string lectureId)
        {
            var courseId = await CreateCourse(lectureCourseClient, lectureId);
            var newHomeworkViewModel = GenerateCreateHomeworkViewModel();
            var newTaskViewModel = GenerateCreateTaskViewModelWithDeadLine();
            var homeworkId = await lectureCourseClient.AddHomeworkToCourse(newHomeworkViewModel, courseId);
            var taskId = await lectureCourseClient.AddTask(newTaskViewModel, homeworkId);
            return (courseId, homeworkId, taskId);
        }

        private CreateGroupViewModel GenerateCreateGroupMateViewModel(string studentId, string studentId2, long courseId, long taskId)
        {
            var groupMateViewModel1 = new GroupMateViewModel {StudentId = studentId};
            var groupMateViewModel2 = new GroupMateViewModel {StudentId = studentId2};
            var list = new List<GroupMateViewModel>
            {
                groupMateViewModel1,
                groupMateViewModel1
            };
            var task = new List<long> {taskId};
            var groupViewModel = new Fixture().Build<CreateGroupViewModel>()
                .With(g => g.GroupMates, list)
                .With(g => g.CourseId, courseId)
                .With(g => g.Tasks, task);
            return groupViewModel.Create();
        }

        [Test]
        public async Task PostAndGetSolutionByIdTest()
        {
            var (studentId, lectureId) = await CreateUserAndLecture();
            var studentCourseClient = CreateCourseServiceClient(studentId);
            var lectureCourseClient = CreateCourseServiceClient(lectureId);
            var (courseId, homeworkId, taskId) = await CreateCourseHomeworkTaskWithOutDeadLine(lectureCourseClient, lectureId);
            await SignStudentInCourse(studentCourseClient, lectureCourseClient, courseId, studentId);
            var solutionClient = CreateSolutionsServiceClient();
            var solutionViewModel = GenerateSolutionViewModel(studentId);
            
            var solutionId = await solutionClient.PostSolution(solutionViewModel, taskId);
            var solutionIdGet = await solutionClient.GetSolutionById(solutionId);

            solutionIdGet.Id.Should().Be(solutionId) ;
        }
        
        [Test]
        public async Task PostAndGetUserSolutionTest()
        {
            var (studentId, lectureId) = await CreateUserAndLecture();
            var studentCourseClient = CreateCourseServiceClient(studentId);
            var lectureCourseClient = CreateCourseServiceClient(lectureId);
            var (courseId, homeworkId, taskId) = await CreateCourseHomeworkTaskWithOutDeadLine(lectureCourseClient, lectureId);
            await SignStudentInCourse(studentCourseClient, lectureCourseClient, courseId, studentId);
            var solutionClient = CreateSolutionsServiceClient();
            var solutionViewModel1 = GenerateSolutionViewModel(studentId);
            var solutionViewModel2 = GenerateSolutionViewModel(studentId);
            
            var solutionId1 = await solutionClient.PostSolution(solutionViewModel1, taskId);
            var solutionId2 = await solutionClient.PostSolution(solutionViewModel2, taskId);
            var solutionIdGet = await solutionClient.GetUserSolution(taskId, studentId);

            solutionIdGet.Should().HaveCount(2);
            solutionIdGet.Should().Contain(s => s.Id == solutionId1 || s.Id == solutionId2);
        }

        [Test]
        public async Task PostAndGetAllSolutionTest()
        {
            var (studentId1, lectureId) = await CreateUserAndLecture();
            var (studentId2, _) = await CreateAndRegisterUser();
            var student1CourseClient = CreateCourseServiceClient(studentId1);
            var student2CourseClient = CreateCourseServiceClient(studentId2);
            var lectureCourseClient = CreateCourseServiceClient(lectureId);
            var (courseId, homeworkId, taskId) = await CreateCourseHomeworkTaskWithOutDeadLine(lectureCourseClient, lectureId);
            await SignStudentInCourse(student1CourseClient, lectureCourseClient, courseId, studentId1);
            await SignStudentInCourse(student2CourseClient, lectureCourseClient, courseId, studentId2);
            var solutionClient = CreateSolutionsServiceClient();
            var solutionViewModelFromStudent1 = GenerateSolutionViewModel(studentId1);
            var solutionViewModelFromStudent2 = GenerateSolutionViewModel(studentId2);
            
            var solutionIdFromStudent1 = await solutionClient.PostSolution(solutionViewModelFromStudent1, taskId);
            var solutionIdFromStudent2 = await solutionClient.PostSolution(solutionViewModelFromStudent2, taskId);
            var solutionsGet = await solutionClient.GetAllSolutions();
            
            solutionsGet.Should().Contain(s => s.Id == solutionIdFromStudent1 || s.Id == solutionIdFromStudent2);
        }
        
        [Test]
        public async Task RateSolutionTest()
        {
            var (studentId, lectureId) = await CreateUserAndLecture();
            var studentCourseClient = CreateCourseServiceClient(studentId);
            var lectureCourseClient = CreateCourseServiceClient(lectureId);
            var (courseId, homeworkId, taskId) = await CreateCourseHomeworkTaskWithOutDeadLine(lectureCourseClient, lectureId);
            await SignStudentInCourse(studentCourseClient, lectureCourseClient, courseId, studentId);
            var solutionClient = CreateSolutionsServiceClient();
            var solutionViewModelFromStudent = GenerateSolutionViewModel(studentId);

            var solutionId = await solutionClient.PostSolution(solutionViewModelFromStudent, taskId);
            await solutionClient.RateSolution(solutionId, 2, "Not Bad", lectureId);
            var solutionsGet = await solutionClient.GetSolutionById(solutionId);

            solutionsGet.LecturerComment.Should().Be("Not Bad");
            solutionsGet.Rating.Should().Be(2);
        }
        
        
        [Test]
        public async Task MarkSolutionTest()
        {
            var (studentId, lectureId) = await CreateUserAndLecture();
            var studentCourseClient = CreateCourseServiceClient(studentId);
            var lectureCourseClient = CreateCourseServiceClient(lectureId);
            var (courseId, homeworkId, taskId) = await CreateCourseHomeworkTaskWithOutDeadLine(lectureCourseClient, lectureId);
            await SignStudentInCourse(studentCourseClient, lectureCourseClient, courseId, studentId);
            var solutionClient = CreateSolutionsServiceClient();
            var solutionViewModelFromStudent = GenerateSolutionViewModel(studentId);

            var solutionId = await solutionClient.PostSolution(solutionViewModelFromStudent, taskId);
            await solutionClient.MarkSolution(solutionId);
            var solutionsGet = await solutionClient.GetSolutionById(solutionId);

            solutionsGet.State.Should().Be(SolutionState.Final);
        }
        
        [Test]
        public async Task DeleteSolutionTest()
        {
            var (studentId, lectureId) = await CreateUserAndLecture();
            var studentCourseClient = CreateCourseServiceClient(studentId);
            var lectureCourseClient = CreateCourseServiceClient(lectureId);
            var (courseId, homeworkId, taskId) = await CreateCourseHomeworkTaskWithOutDeadLine(lectureCourseClient, lectureId);
            await SignStudentInCourse(studentCourseClient, lectureCourseClient, courseId, studentId);
            var solutionClient = CreateSolutionsServiceClient();
            var solutionViewModel = GenerateSolutionViewModel(studentId);
            
            var solutionId = await solutionClient.PostSolution(solutionViewModel, taskId);
            var solutionIdBefore = await solutionClient.GetSolutionById(solutionId);
            await solutionClient.DeleteSolution(solutionId);
            var solutionIdAfter = await solutionClient.GetUserSolution(taskId, studentId);

            solutionIdBefore.Id.Should().Be(solutionId);
            solutionIdAfter.Should().NotContain(s => s.Id == solutionIdBefore.Id);
        }

        [Test]
        public async Task GetCourseStatisticsTest()
        {
            var (studentId, lectureId) = await CreateUserAndLecture();
            var studentCourseClient = CreateCourseServiceClient(studentId);
            var lectureCourseClient = CreateCourseServiceClient(lectureId);
            var (courseId, homeworkId, taskId) = await CreateCourseHomeworkTaskWithOutDeadLine(lectureCourseClient, lectureId);
            await SignStudentInCourse(studentCourseClient, lectureCourseClient, courseId, studentId);
            var solutionClient = CreateSolutionsServiceClient();
            var solutionViewModel = GenerateSolutionViewModel(studentId);
            
            var solutionId = await solutionClient.PostSolution(solutionViewModel, taskId);
            await solutionClient.RateSolution(solutionId, 2, "Not Bad", lectureId);
            var statisticsFromStudent = await solutionClient.GetCourseStatistics(courseId, studentId);
            var statisticsFromLecture = await solutionClient.GetCourseStatistics(courseId, lectureId);
            
            statisticsFromStudent.Should().BeNull();
            statisticsFromLecture.Should().HaveCount(1);
            statisticsFromLecture[0].Homeworks.Should().HaveCount(1);
            statisticsFromLecture[0].Homeworks[0].Tasks.Should().HaveCount(1);
            statisticsFromLecture[0].Homeworks[0].Tasks[0].Solution.Should().HaveCount(1);
            statisticsFromLecture[0].Homeworks[0].Tasks[0].Solution[0].Rating.Should().Be(2);
        }

        [Test]
        public async Task TryPostSolutionAfterDeadLine()
        {
            var (studentId, lectureId) = await CreateUserAndLecture();
            var studentCourseClient = CreateCourseServiceClient(studentId);
            var lectureCourseClient = CreateCourseServiceClient(lectureId);
            var courseId = await CreateCourse(lectureCourseClient, lectureId);
            var homeworkViewModel =  GenerateCreateHomeworkViewModel();
            var homeworkId = await lectureCourseClient.AddHomeworkToCourse(homeworkViewModel, courseId);
            var taskViewModel = GenerateCreateTaskViewModelWithDeadLine();
            var taskId = await lectureCourseClient.AddTask(taskViewModel, homeworkId);
            var solutionsClient = CreateSolutionsServiceClient();
            
            var solutionViewModel = GenerateSolutionViewModel(studentId);
            solutionViewModel.PublicationDate = DateTime.MaxValue;

            Assert.ThrowsAsync<ForbiddenException>(async () => await solutionsClient.PostSolution(solutionViewModel, taskId));
        }

        /*[Test]
        public async Task PostGroupSolutionTest()
        {
            var (studentId, lectureId) = await CreateUserAndLecture();
            var (studentId1, _) = await CreateAndRegisterUser();
            var studentCourseClient = CreateCourseServiceClient(studentId);
            var lectureCourseClient = CreateCourseServiceClient(lectureId);
            var (courseId, homeworkId, taskId) = await CreateCourseHomeworkTask(lectureCourseClient, lectureId);
            await SignStudentInCourse(studentCourseClient, lectureCourseClient, courseId, studentId);
            var solutionClient = CreateSolutionsServiceClient();
            var groupViewModel = GenerateCreateGroupMateViewModel(studentId, studentId1, courseId, taskId);
            var groupId = await lectureCourseClient.CreateCourseGroup(groupViewModel, courseId);
            var solutionViewModel = GenerateSolutionViewModel(studentId);
            var solutionViewModel1 = GenerateSolutionViewModel(studentId1);
            
            var solutionId = await solutionClient.PostGroupSolution(solutionViewModel, taskId, groupId);
            var solutionId1 = await solutionClient.PostGroupSolution(solutionViewModel1, taskId, groupId);
            var solution = await solutionClient.GetSolutionById(solutionId);
            var solution1 = await solutionClient.GetSolutionById(solutionId1);
            
            
            solution.GroupId.Should().Be(groupId);
            solution1.GroupId.Should().Be(groupId);
        }

        [Test]
        public async Task GetTaskSolutionsTest()
        {
            var (studentId, lectureId) = await CreateUserAndLecture();
            var (studentId1, _) = await CreateAndRegisterUser();
            var studentCourseClient = CreateCourseServiceClient(studentId);
            var lectureCourseClient = CreateCourseServiceClient(lectureId);
            var (courseId, homeworkId, taskId) = await CreateCourseHomeworkTask(lectureCourseClient, lectureId);
            var taskViewModel2 = GenerateCreateTaskViewModel();
            var solutionClient = CreateSolutionsServiceClient();
            var groupViewModel = GenerateCreateGroupMateViewModel(studentId, studentId1, courseId, taskId);
            var groupId = await lectureCourseClient.CreateCourseGroup(groupViewModel, courseId);
            var solutionViewModelForTask1 = GenerateSolutionViewModel(studentId);
            var solutionViewModelForTask2 = GenerateSolutionViewModel(studentId);

            var solutionId1 = await solutionClient.PostSolution(solutionViewModelForTask1, taskId);
            var solutionId2 = await solutionClient.PostSolution(solutionViewModelForTask2, taskId);

            var solution = await solutionClient.GetTaskSolutions(groupId, taskId);

            solution.Should().HaveCount(2);
            solution.Should().Contain(s => s.Id == solutionId1 || s.Id == solutionId2);
        }*/
    }
}