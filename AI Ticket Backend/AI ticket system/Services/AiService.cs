using System;
using System.Text;
using System.Text.Json;
using AI_ticket_system.Models;
using AI_ticket_system.Services.IServices;
using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Web;
using Newtonsoft.Json;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace AI_ticket_system.Services
{
    public class AiService : IAiService
    {
        private readonly IGenerativeModelService _geminiService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;            
        public AiService(HttpClient httpClient, IGenerativeModelService geminiService, IConfiguration config)
        {
            _geminiService = geminiService;
            _httpClient = httpClient;
            _config = config;
        }


        public async Task<AiResponse?> AnalyzeTicketAsync(Ticket ticket)
        {
            var prompt = @$"
        You are a ticket triage agent. Only return a strict JSON object with no extra text, headers, or markdown.

        Analyze the following support ticket and provide a JSON object with:
            - summary
            - priority (low|medium|high)
            - helpfulNotes
            - relatedSkills

        Ticket:
            Title: {ticket.Title}
            Description: {ticket.Description}";

            var apiKey = _config["Gemini:Credentials:ApiKey"];

            var request = new ContentRequest
            {
                contents = new[]
                {
                    new AI_ticket_system.Models.Content
                    {
                        parts = new[]
                        {
                            new AI_ticket_system.Models.Part
                            {
                                text = prompt
                            }
                        }
                    }
                }
            };

            try
            {
                string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";
                string jsonRequest = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                //var result = await _geminiService.GenerateContent(prompt);
                //var jsonString = result.Text?.Trim().Trim('`');

                if (string.IsNullOrWhiteSpace(jsonRequest))
                    return null;

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                    var json = JsonDocument.Parse(jsonResponse);
                    var rawOutput = json.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    var jsonString = rawOutput.Replace("```json", "").Replace("```", "").Trim();
                    var geminiResponse = JsonConvert.DeserializeObject<AiResponse>(jsonString);

                    var result = new AiResponse()
                    {
                        Summary = geminiResponse?.Summary ?? "",
                        Priority = geminiResponse?.Priority ?? "",
                        HelpfulNotes = geminiResponse?.HelpfulNotes ?? "",
                        RelatedSkills = geminiResponse?.RelatedSkills ?? new List<string>()
                    };

                    return result;

                }
                else
            {
                throw new Exception("Error communicating with Gemini API.");
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error analyzing ticket: " + ex.Message);
                return null;
            }
        }

    }
}
