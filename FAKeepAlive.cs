using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace fa_keep_alive_test
{
    public class FAKeepAlive
    {
        [FunctionName("fa_keep_alive")]
        public void Run([TimerTrigger("%ScheduleExpression%")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            Task.Run(async () =>
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("https://foo.com");

                    if (response != null && response.IsSuccessStatusCode)
                    {
                        log.LogInformation(response.ToString());
                        if (response.Content != null)
                        {
                            log.LogInformation(await response.Content.ReadAsStringAsync());
                        }
                    }
                    else
                    {
                        log.LogError(response?.ToString());
                    }
                }
            }).Wait();
        }
    }
}