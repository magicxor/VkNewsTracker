using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Citrina;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using VkNewsTracker.Common.Constants;

namespace VkNewsTracker.Common.Services
{
    public class NewsService
    {
        private readonly SettingsService _settingsService;
        private readonly CitrinaClient _vkClient;
        private readonly UserAccessToken _vkAccessToken;
        private readonly ILogger _logger;
        private readonly ITelegramBotClient _botClient;

        public NewsService(SettingsService settingsService, CitrinaClient vkClient, UserAccessToken vkAccessToken, ILogger logger, ITelegramBotClient botClient)
        {
            _settingsService = settingsService;
            _vkClient = vkClient;
            _vkAccessToken = vkAccessToken;
            _logger = logger;
            _botClient = botClient;
        }

        private ApiRequest<NewsfeedGetResponse> FetchUpdatesStartingFrom(string startFrom = null)
        {
            var apiCall = _vkClient
                .Newsfeed
                .Get(_vkAccessToken,
                    startFrom: string.IsNullOrEmpty(startFrom) ? null : startFrom, 
                    count: Defaults.MaxNewsfeedSize)
                .Result;
            return apiCall;
        }


        public void FetchUpdates()
        {
            const int maxTelegramStringLength = 4095;
            var postsIdentifiers = new HashSet<string>();
            NewsfeedNewsfeedItem lastItem = null;
            string startFrom = null;

            while (lastItem == null
                || (!string.IsNullOrEmpty(startFrom)
                    && lastItem.Date.GetValueOrDefault().ToUniversalTime() > _settingsService.ApplicationSettings.LastUpdate))
            {
                _logger.LogTrace($"{nameof(startFrom)}={startFrom}. Date of the last item = {lastItem?.Date.GetValueOrDefault().ToUniversalTime()}");
                var apiCall = FetchUpdatesStartingFrom(startFrom);
                if (!apiCall.IsError)
                {
                    foreach (var responseItem in apiCall.Response.Items)
                    {
                        if (!string.IsNullOrEmpty(responseItem.Text)
                            && responseItem.Date.GetValueOrDefault().ToUniversalTime() > _settingsService.ApplicationSettings.LastUpdate
                            && !postsIdentifiers.Contains($"{responseItem.SourceId}_{responseItem.PostId}")
                            && _settingsService.ApplicationSettings.IncludedPatterns.Any(pattern => Regex.IsMatch(responseItem.Text, pattern))
                            && !_settingsService.ApplicationSettings.ExcludedPatterns.Any(pattern => Regex.IsMatch(responseItem.Text, pattern)))
                        {
                            postsIdentifiers.Add($"{responseItem.SourceId}_{responseItem.PostId}");
                            var messageText = $"New match found: {responseItem.Date} https://vk.com/wall{responseItem.SourceId}_{responseItem.PostId} {responseItem.Text}";
                            if (messageText.Length > maxTelegramStringLength)
                            {
                                messageText = messageText.Substring(0, maxTelegramStringLength);
                            }
                            var message = _botClient.SendTextMessageAsync(new ChatId(_settingsService.ApplicationSettings.TelegramChatId), messageText).Result;
                            Thread.Sleep(TimeSpan.FromMilliseconds(_settingsService.ApplicationSettings.TelegramMessageFrequency));
                        }
                    }
                    lastItem = apiCall.Response.Items.Where(item => item.Date.HasValue).OrderByDescending(item => item.Date.Value).LastOrDefault();
                    startFrom = apiCall.Response.NextFrom;
                    Thread.Sleep(TimeSpan.FromMilliseconds(_settingsService.ApplicationSettings.VkPageFetchFrequency));
                }
                else
                {
                    var errorMessage = apiCall.Error.Message;
                    throw new Exception(errorMessage);
                }
            }
            _logger.LogTrace($"{nameof(startFrom)}={startFrom}. Date of the last item = {lastItem?.Date.GetValueOrDefault().ToUniversalTime()}. Fetching is stopped.");
            _settingsService.ApplicationSettings.LastUpdate = DateTime.UtcNow;
            _settingsService.Save();
        }
    }
}
