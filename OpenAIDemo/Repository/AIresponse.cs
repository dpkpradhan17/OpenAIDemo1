
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using OpenAIDemo.Models;

namespace WebApplication1.Repository
{
    public class AIresponse
    {
        public async Task<string> GetResponse(string prompt, string key)
        {
            //string apiKey = "Your API key here";
            string apiKey = key;    

            try
            {
                using HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

                string model = "gpt-3.5-turbo"; // Specify the model
                string apiUrl = "https://api.openai.com/v1/chat/completions";

                var requestData = new
                {
                    model = model,
                    messages = new[]
                    {
                    new { role = "user", content = prompt }
                },
                    temperature = 0.7
                };

                string requestDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
                var content = new StringContent(requestDataJson, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(apiUrl, content);
                string responseBody = await response.Content.ReadAsStringAsync();
                var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseJson>(responseBody);

                if (response.IsSuccessStatusCode)
                {

                    var contentPart = jsonResponse.Choices[0].Message.Content;
                    Console.WriteLine(contentPart);

                    return contentPart;
                }
                else
                {
                    return "Error: " + responseBody;
                }
            }
            catch (Exception ex)
            {
                return "An Error occurred: " + ex.Message;
            }

        }
    }
}
