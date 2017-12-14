using System.Reflection;
using Citrina;
using Microsoft.Extensions.Logging;
using VkNewsTracker.Common.Constants;
using VkNewsTracker.Common.Services;

namespace VkNewsTracker.Common
{
    public class Worker
    {
        private readonly SettingsService _settingsService;
        private readonly CitrinaClient _vkClient;
        private readonly NewsService _newsService;
        private readonly ILogger _logger;

        public Worker(SettingsService settingsService, CitrinaClient vkClient, NewsService newsService, ILogger logger)
        {
            _settingsService = settingsService;
            _vkClient = vkClient;
            _newsService = newsService;
            _logger = logger;
        }

        public void Run()
        {
            _logger.LogDebug("{0} has been started", Assembly.GetEntryAssembly().GetName().Name);

            if (string.IsNullOrEmpty(_settingsService.ApplicationSettings.AccessToken) || _settingsService.ApplicationSettings.UserId == default(int))
            {
                _logger.LogInformation("Please obtain AccessToken and UserId: " + _vkClient.AuthHelper.GenerateLink(LinkType.Token,
                                 _settingsService.ApplicationSettings.ApplicationId,
                                 Defaults.RedirectUri,
                                 DisplayOptions.Default,
                                 UserPermissions.Wall | UserPermissions.Friends | UserPermissions.Offline,
                                 ""));
            }
            else
            {
                _newsService.FetchUpdates();
            }

            _logger.LogDebug("{0} has been stopped", Assembly.GetEntryAssembly().GetName().Name);
        }
    }
}
