﻿using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HwProj.Models.NotificationsService;
using HwProj.NotificationsService.API.Repositories;
using HwProj.TelegramBotService.Client;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace HwProj.NotificationsService.API.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfigurationSection _configuration;
        private readonly TelegramBotClient _botClient;
        private readonly ITelegramBotServiceClient _telegramBotServiceClient;

        public NotificationsService(INotificationsRepository repository, IMapper mapper, IConfiguration configuration, ITelegramBotServiceClient telegramBotServiceClient)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration.GetSection("Telegram");
            _telegramBotServiceClient = telegramBotServiceClient;
            _botClient = new TelegramBotClient(_configuration["Token"]);
        }

        public async Task<long> AddNotificationAsync(Notification notification)
        {
            var id = await _repository.AddAsync(notification);
            return id;
        }

        public async Task<NotificationViewModel[]> GetAsync(string userId, NotificationFilter filter = null)
        {
            filter = filter ?? new NotificationFilter
            {
                MaxCount = 50, 
            };
            var notifications = await _repository.GetAllByUserAsync(userId, filter);
            return notifications.Select(notification => _mapper.Map<NotificationViewModel>(notification)).ToArray();
        }

        public async Task MarkAsSeenAsync(string userId, long[] notificationIds)
        {
            await _repository.UpdateBatchAsync(userId, notificationIds,
                t => new Notification {HasSeen = true});
        }
        
        public async Task SendTelegramMessageAsync(Notification notification, InlineKeyboardMarkup inlineKeyboard)
        {
            var user = await _telegramBotServiceClient.GetTelegramUser(notification.Owner);
            if (user != null)
            {
                if (inlineKeyboard == null)
                {
                    await _botClient.SendTextMessageAsync(user.ChatId, notification.Body, ParseMode.Markdown);
                }
                else
                {
                    await _botClient.SendTextMessageAsync(user.ChatId, notification.Body, ParseMode.Markdown, replyMarkup:inlineKeyboard);
                }
            }
        }
    }
}