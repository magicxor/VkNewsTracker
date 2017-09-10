using Autofac;
using Citrina;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using VkNewsTracker.Common.Services;

namespace VkNewsTracker.Common.DependencyInjection
{
    public class ContainerConfig
    {
        public static IContainer Configure(string applicationSettingsPath)
        {
            var builder = new ContainerBuilder();

            var settingsService = new SettingsService(applicationSettingsPath);
            settingsService.Load();

            var vkClient = new CitrinaClient();
            var vkAccessToken = new UserAccessToken(settingsService.ApplicationSettings.AccessToken,
                settingsService.ApplicationSettings.AccessTokenLifetime,
                settingsService.ApplicationSettings.UserId,
                settingsService.ApplicationSettings.ApplicationId);

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(LogLevel.Trace);
            loggerFactory.AddFile("{Date}.txt", LogLevel.Trace);

            var botClient = new TelegramBotClient(settingsService.ApplicationSettings.TelegramBotToken);

            builder.RegisterInstance(settingsService).As<SettingsService>().ExternallyOwned();
            builder.RegisterInstance(vkClient).As<CitrinaClient>().ExternallyOwned();
            builder.RegisterInstance(vkAccessToken).As<UserAccessToken>().ExternallyOwned();
            builder.RegisterInstance(loggerFactory.CreateLogger("")).As<ILogger>().ExternallyOwned();
            builder.RegisterInstance(botClient).As<ITelegramBotClient>().ExternallyOwned();

            builder.RegisterType<NewsService>().As<NewsService>();
            builder.RegisterType<Worker>().As<Worker>();
            var container = builder.Build();
            return container;
        }
    }
}
