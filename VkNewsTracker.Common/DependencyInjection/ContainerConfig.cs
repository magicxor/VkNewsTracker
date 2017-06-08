using Autofac;
using Citrina;
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

            builder.RegisterInstance(settingsService).As<SettingsService>().ExternallyOwned();
            builder.RegisterInstance(vkClient).As<CitrinaClient>().ExternallyOwned();
            builder.RegisterInstance(vkAccessToken).As<UserAccessToken>().ExternallyOwned();
            builder.RegisterType<NewsService>().As<NewsService>();
            builder.RegisterType<Worker>().As<Worker>();
            var container = builder.Build();
            return container;
        }
    }
}
