using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;

namespace LearningAzure
{
    public static class SendMessage
    {
        [FunctionName("SendMessage")]
        [return: ServiceBus("new-queue", ServiceBusEntityType.Queue)]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string body;
            StreamReader reader;

            using (reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                try
                {
                    body = await reader.ReadToEndAsync();
                    log.LogInformation($"Message body : {body}");
                    return body;
                }
                catch (Exception ex)
                {
                    log.LogError(ex.ToString());
                    return ex.Message;
                }

            }
        }
    }
}
