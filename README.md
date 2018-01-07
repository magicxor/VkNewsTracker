# VkNewsTracker
Track your vk.com newsfeed updates with regex filtering and telegram notifications

# Configuration
## appsettings.json:
```json
{
   "TelegramMessageFrequency":300,
   "TelegramBotToken":"YOUR_TOKEN",
   "TelegramChatId":-XXXXXXX,
   "VkPageFetchFrequency":1000,
   "ApplicationId":5891549,
   "AccessToken":"YOUR_TOKEN",
   "AccessTokenLifetime":3600,
   "UserId":"YOUR_USER_ID",
   "RefreshPeriod":3600,
   "LastUpdate":"2017-01-08T05:38:19.4728478Z",
   "IncludedPatterns":[
      ".*London.*",
      ".*Москва.*"
   ],
   "ExcludedPatterns":[

   ]
}
```

# Depedencies
* .NET Core 2.0
* Microsoft.Extensions.Logging with Microsoft.Extensions.Logging.Console and Serilog.Extensions.Logging.File providers
* [Citrina](https://github.com/khrabrovart/Citrina)
* [Autofac](https://autofac.org)
* [Telegram.Bot](https://github.com/TelegramBots/telegram.bot)
