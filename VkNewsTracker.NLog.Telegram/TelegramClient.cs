using System;
using System.Net.Http;

namespace NLog.Telegram
{
    public class TelegramClient
    {
        public event Action<Exception> Error;

        public void Send(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var result = client.PostAsync(new Uri(url), new StringContent("GET")).Result;
                }
            }
            catch (Exception e)
            {
                this.OnError(e);
            }
        }

        private void OnError(Exception obj)
        {
            if (this.Error != null)
                this.Error(obj);
        }
    }
}