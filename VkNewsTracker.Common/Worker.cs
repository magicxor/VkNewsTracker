using Citrina;
using NLog;
using VkNewsTracker.Common.Constants;
using VkNewsTracker.Common.Services;

namespace VkNewsTracker.Common
{
    public class Worker
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SettingsService _settingsService;
        private readonly CitrinaClient _vkClient;
        private readonly NewsService _newsService;

        public Worker(SettingsService settingsService, CitrinaClient vkClient, NewsService newsService)
        {
            _settingsService = settingsService;
            _vkClient = vkClient;
            _newsService = newsService;
        }

        public void Run()
        {
            if (string.IsNullOrEmpty(_settingsService.ApplicationSettings.AccessToken) || _settingsService.ApplicationSettings.UserId == default(int))
            {
                _logger.Info("Please obtain AccessToken and UserId: " + _vkClient.AuthHelpers.GenerateLink(LinkType.Token,
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
        }
    }
}
