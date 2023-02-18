using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace fa_keep_alive_test
{
    public class FAKeepAlive
    {
        [FunctionName("fa_keep_alive")]
        public async Task RunAsync([TimerTrigger("%ScheduleExpression%")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var websites = new List<Website>();

            var config = new ConfigurationBuilder()
               .SetBasePath(context.FunctionAppDirectory)
               .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

            int i = 0;
            string url = config.GetSection($"Websites:Urls:{i}").Value;

            while (url != null)
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

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

                url = config.GetSection($"Websites:Urls:{++i}").Value;
            }       

        }

        private struct Website
        {
            string Url { get; set; }
        }
    }
}