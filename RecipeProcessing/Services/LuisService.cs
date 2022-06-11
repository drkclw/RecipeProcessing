using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Newtonsoft.Json;

namespace RecipeProcessing.Services
{
    public class LuisService
    {
        private IConfiguration _configuration;

        public LuisService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public LUISRuntimeClient Authenticate()
        {
            var languageUnderstandingEndpoint = _configuration["languageUnderstandingEndpoint"];
            var languageUnderstandingKey = _configuration["languageUnderstandingKey"];
            var credentials = new ApiKeyServiceClientCredentials(languageUnderstandingKey);
            return new LUISRuntimeClient(credentials) { Endpoint = languageUnderstandingEndpoint };
        }

        public async Task<string> GetIntent(string query)
        {
            var runtimeClient = Authenticate();
            var request = new PredictionRequest { Query = query };
            var appId = new Guid(_configuration["languageUnderstandingAppId"]);
            var prediction = await runtimeClient.Prediction.GetSlotPredictionAsync(appId, "Staging", request);
            var json = JsonConvert.SerializeObject(prediction, Formatting.Indented);

            Console.WriteLine(json);
            return prediction.Prediction.TopIntent;
        }
    }
}
