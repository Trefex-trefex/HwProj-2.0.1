using System;
using System.Threading.Tasks;
using HwProj.EventBus.Client.Interfaces;
using HwProj.Models.NotificationsService;
using HwProj.NotificationsService.API.Repositories;
using HwProj.NotificationsService.API.Services;
using HwProj.SolutionsService.API.Events;

namespace HwProj.NotificationsService.API.EventHandlers
{
    // ReSharper disable once UnusedType.Global
    public class RateEventHandler : IEventHandler<RateEvent>
    {
        private readonly INotificationsRepository _notificationRepository;
        private readonly INotificationsService _notificationsService;

        public RateEventHandler(INotificationsRepository notificationRepository, INotificationsService notificationsService)
        {
            _notificationRepository = notificationRepository;
            _notificationsService = notificationsService;
        }

        public async Task HandleAsync(RateEvent @event)
        {
            var notification = new Notification
            {
                Sender = "SolutionService",
                Body = $"Задача <a href='task/{@event.Task.Id}' target='_blank'>{@event.Task.Title}</a> оценена.",
                Category = "SolutionService",
                Date = DateTime.UtcNow,
                HasSeen = false,
                Owner = @event.Solution.StudentId
            };
            await _notificationRepository.AddAsync(notification);
            await _notificationsService.SendTelegramMessageAsync(notification);
        }
    }
}