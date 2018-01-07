using System.IO;
using Autofac;
using Citrina;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using VkNewsTracker.Common.Constants;
using VkNewsTracker.Common.Services;

namespace VkNewsTracker.Common.DependencyInjection
{
    public class ContainerConfiguration
    {
        public static IContainer Configure(string applicationPath)
        {
            var builder = new ContainerBuilder();

            var settingsService = new SettingsService(Path.Combine(applicationPath, Defaults.ConfigurationFileName));
            settingsService.Load();

            var vkClient = new CitrinaClient();
            var vkAccessToken = new UserAccessToken(settingsService.ApplicationSettings.AccessToken,
                settingsService.ApplicationSettings.AccessTokenLifetime,
                settingsService.ApplicationSettings.UserId,
                settingsService.ApplicationSettings.ApplicationId);

            var loggerFactory = new LoggerFactory();
            var logPath = Path.Combine(applicationPath, Defaults.LogDirectoryName, "log-{Date}.txt");
            loggerFactory.AddConsole(LogLevel.Trace).AddFile(logPath, LogLevel.Trace, retainedFileCountLimit: 10);

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
