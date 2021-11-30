﻿using HwProj.EventBus.Client;
using HwProj.TelegramBotService.API.Models;

namespace HwProj.TelegramBotService.API.Events
{
    public class ConfirmTelegramBotEvent : Event
    {
        public TelegramUserModel TelegramUserModel { get; set; }

        public ConfirmTelegramBotEvent(TelegramUserModel userModel)
        {
            TelegramUserModel = userModel;
        }
    }
}