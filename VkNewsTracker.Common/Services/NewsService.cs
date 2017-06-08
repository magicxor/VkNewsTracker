using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Citrina;
using Citrina.StandardApi;
using Citrina.StandardApi.Models;
using NLog;

namespace VkNewsTracker.Common.Services
{
    public class NewsService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SettingsService _settingsService;
        private readonly CitrinaClient _vkClient;
        private readonly UserAccessToken _vkAccessToken;

        public NewsService(SettingsService settingsService, CitrinaClient vkClient, UserAccessToken vkAccessToken)
        {
            _settingsService = settingsService;
            _vkClient = vkClient;
            _vkAccessToken = vkAccessToken;
        }

        public async Task<ApiCall<NewsfeedGetRequest, NewsfeedGetResponse>> FetchUpdatesAsync()
        {
            var apiCall = await _vkClient.Newsfeed.Get(new NewsfeedGetRequest()
            {
                AccessToken = _vkAccessToken,
                Count = 100,
                StartTime = _settingsService.ApplicationSettings.LastUpdate,
            }).ConfigureAwait(false);
            return apiCall;
        }


        public void FetchUpdates()
        {
            var apiCall = FetchUpdatesAsync().Result;

            if (!apiCall.IsError)
            {
                foreach (var responseItem in apiCall.Response.Items)
                {
                    if (!string.IsNullOrEmpty(responseItem.Text) &&
                        _settingsService.ApplicationSettings.IncludedPatterns.Any(
                            pattern => Regex.IsMatch(responseItem.Text, pattern)) &&
                        !_settingsService.ApplicationSettings.ExcludedPatterns.Any(
                            pattern => Regex.IsMatch(responseItem.Text, pattern)))
                    {
                        _logger.Info($"New match found: {responseItem.Date} https://vk.com/wall{responseItem.SourceId}_{responseItem.PostId} {responseItem.Text}");
                    }
                }

                _settingsService.ApplicationSettings.LastUpdate = DateTime.UtcNow;
                _settingsService.Save();
            }
            else
            {
                var errorMessage = apiCall.Error.Message;
                throw new Exception(errorMessage);
            }
        }
    }
}
