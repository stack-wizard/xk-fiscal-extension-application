using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Mikos.XK.Fiscal.Dtos;

namespace Mikos.XK.Fiscal.Services
{
    public class ApiHelper
    {
        public static async Task<FiscalResponseData> PostAsync(FiscalRequestData content, string url)
        {
            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    var json = JsonConvert.SerializeObject(content);
                    using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        request.Content = stringContent;
                        using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(true))
                        {
                            var contents = await response.Content.ReadAsStringAsync();


                            FiscalResponseData responseModel = JsonConvert.DeserializeObject<FiscalResponseData>(contents);

                            return responseModel;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return toNoFiscalResponse();
            }
        }

        private static FiscalResponseData toNoFiscalResponse()
        {
            FiscalResponseData data = new FiscalResponseData()
            {
                FiscalOutputs = new FiscalOutputs(),
                StatusMessages = toNoResponseStatusMessages()
            };


            return data;
        }

        private static StatusMessages toNoResponseStatusMessages()
        {
            StatusMessages statusMessages = new StatusMessages();
            List<Message> Messages = new List<Message>();

            Messages.Add(new Message()
            {
                Type = "Error",
                Description = "The fiscal service is unavailable. Please contact customer support."
            });

            statusMessages.Messages = Messages;


            return statusMessages;
        }
    }
}
