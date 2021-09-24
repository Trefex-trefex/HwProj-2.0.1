using System;
using System.Threading.Tasks;
using AutoMapper;
using HwProj.AuthService.API.Events;
using HwProj.AuthService.Client;
using HwProj.EventBus.Client.Interfaces;
using HwProj.Models.NotificationsService;
using HwProj.NotificationsService.API.Repositories;
using HwProj.NotificationsService.API.Services;
using HwProj.SolutionsService.API.Events;
using Microsoft.Extensions.Configuration;

namespace HwProj.NotificationsService.API.EventHandlers
{
    // ReSharper disable once UnusedType.Global
    public class StudentPassTaskEventHandler : IEventHandler<StudentPassTaskEvent>
    {
        private readonly INotificationsRepository _notificationRepository;
        private readonly INotificationsService _notificationsService;
        private readonly IAuthServiceClient _authServiceClient;
        private readonly IMapper _mapper;
        private readonly IConfigurationSection _configuration;

        public StudentPassTaskEventHandler(INotificationsRepository notificationRepository,
            INotificationsService notificationsService,
            IMapper mapper,
            IAuthServiceClient authServiceClient,
            IConfiguration configuration
        )
        {
            _notificationsService = notificationsService;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _authServiceClient = authServiceClient;
            _configuration = configuration.GetSection("Notification");
        }

        public async Task HandleAsync(StudentPassTaskEvent @event)
        {
            foreach (var m in @event.Course.MentorIds.Split('/'))
            {
                var notification = new Notification
                {
                    Sender = "SolutionService",
                    Body = $"{@event.Student.Name} {@event.Student.Surname} добавил новое " +
                           $"<a href='{@event.Solution.GithubUrl}' target='_blank'>решение</a>" +
                           $" задачи <a href='{_configuration["Url"]}/task/{@event.Task.Id}'>{@event.Task.Title}</a>" +
                           $" из курса <a href='{_configuration["Url"]}/courses/{@event.Course.Id}'>{@event.Course.Name}</a>.",
                    Category = "SolutionService",
                    Date = DateTime.UtcNow,
                    HasSeen = false,
                    Owner = m
                };
            
                var mentor = await _authServiceClient.GetAccountData(notification.Owner);
                
                await _notificationRepository.AddAsync(notification);
                await _notificationsService.SendEmailAsync(notification, mentor.Email, "HwProj");
            }
        }
    }
}