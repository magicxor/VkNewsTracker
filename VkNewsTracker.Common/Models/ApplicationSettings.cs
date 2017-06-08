using System;
using System.Collections.Generic;
using VkNewsTracker.Common.Constants;

namespace VkNewsTracker.Common.Models
{
    public class ApplicationSettings
    {
        public int ApplicationId { get; set; } = Defaults.ApplicationId;
        public string AccessToken { get; set; } = string.Empty;
        public int AccessTokenLifetime { get; set; } = Defaults.AccessTokenLifetime;
        public int UserId { get; set; } = default(int);
        public int RefreshPeriod { get; set; } = Defaults.RefreshPeriod;
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow.AddMonths(-2);
        public List<string> IncludedPatterns { get; set; } = new List<string>();
        public List<string> ExcludedPatterns { get; set; } = new List<string>();
    }
}
