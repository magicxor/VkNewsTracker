using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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

        private HashSet<string> _postsIdentifiers = new HashSet<string>();

        public NewsService(SettingsService settingsService, CitrinaClient vkClient, UserAccessToken vkAccessToken)
        {
            _settingsService = settingsService;
            _vkClient = vkClient;
            _vkAccessToken = vkAccessToken;
        }

        public async Task<ApiCall<NewsfeedGetRequest, NewsfeedGetResponse>> FetchUpdatesAsync(string startFrom = null)
        {
            var apiCall = await _vkClient.Newsfeed.Get(new NewsfeedGetRequest()
            {
                AccessToken = _vkAccessToken,
                Count = 100,
                StartFrom = string.IsNullOrEmpty(startFrom) ? null : startFrom,
            }).ConfigureAwait(false);
            return apiCall;
        }


        public void FetchUpdates(string startFrom = null)
        {
            var apiCall = FetchUpdatesAsync(startFrom).Result;

            if (!apiCall.IsError)
            {
                foreach (var responseItem in apiCall.Response.Items)
                {
                    if (!string.IsNullOrEmpty(responseItem.Text) &&
                        responseItem.Date.GetValueOrDefault().ToUniversalTime() > _settingsService.ApplicationSettings.LastUpdate &&
                        !_postsIdentifiers.Contains($"{responseItem.SourceId}_{responseItem.PostId}") &&
                        _settingsService.ApplicationSettings.IncludedPatterns.Any(
                            pattern => Regex.IsMatch(responseItem.Text, pattern)) &&
                        !_settingsService.ApplicationSettings.ExcludedPatterns.Any(
                            pattern => Regex.IsMatch(responseItem.Text, pattern)))
                    {
                        _postsIdentifiers.Add($"{responseItem.SourceId}_{responseItem.PostId}");
                        _logger.Info($"New match found: {responseItem.Date} https://vk.com/wall{responseItem.SourceId}_{responseItem.PostId} {responseItem.Text}");
                       Thread.Sleep(300);
                    }
                }

                if (!string.IsNullOrEmpty(apiCall.Response.NextFrom) &&
                    apiCall.Response.Items.LastOrDefault() != null &&
                    apiCall.Response.Items.LastOrDefault().Date.GetValueOrDefault().ToUniversalTime() >
                    _settingsService.ApplicationSettings.LastUpdate)
                {
                    FetchUpdates(apiCall.Response.NextFrom);
                    return;
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
