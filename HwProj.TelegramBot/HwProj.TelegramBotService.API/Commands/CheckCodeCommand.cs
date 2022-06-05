﻿using System.Threading.Tasks;
using HwProj.TelegramBotService.API.Service;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace HwProj.TelegramBotService.API.Commands
{
    public class CheckCodeCommand : Commands
    {
        private readonly TelegramBotClient _botClient;
        private readonly IUserTelegramService _userTelegramService;

        public CheckCodeCommand(TelegramBot telegramBot, IUserTelegramService userTelegramService)
        {
            _botClient = telegramBot.GetBot().Result;
            _userTelegramService = userTelegramService;
        }

        public override string Name => CommandNames.CheckCodeCommand;
        
        public override async Task ExecuteAsync(Update update)
        {
            var user = await _userTelegramService.AddFinishUser(update.Message.Chat.Id, update.Message.Text);
            
            var inlineKeyboard = new InlineKeyboardMarkup(GetButton("Мои курсы", "/courses"));

            await _botClient.SendTextMessageAsync(user.ChatId, "Добро пожаловать в Hw-ProjBot!", replyMarkup: inlineKeyboard);
        }
    }
}