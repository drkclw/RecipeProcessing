using Azure;
using Azure.AI.TextAnalytics;
using RecipeProcessing.Models.TextAnalytics;

namespace RecipeProcessing.Services
{
    public class TextAnalyticsService
    {
        private readonly IConfiguration _configuration;

        public TextAnalyticsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private TextAnalyticsClient Authenticate()
        {
            var textEndpoint = _configuration["textAnalyticsEndpoint"];
            var textKey = _configuration["textAnalyticsKey"];

            var client =
              new TextAnalyticsClient(new Uri(textEndpoint), new AzureKeyCredential(textKey));
            return client;
        }

        public async Task<EntitiesResult> RecognizeEntities(string phrase)
        {
            var textAnalyticsClient = Authenticate();
            var response = await textAnalyticsClient.RecognizeEntitiesAsync(phrase);

            return new EntitiesResult
            {
                Phrase = phrase,
                Categories = response.Value.Select(entity => entity.Category.ToString())
            };
        }   
    }
}
